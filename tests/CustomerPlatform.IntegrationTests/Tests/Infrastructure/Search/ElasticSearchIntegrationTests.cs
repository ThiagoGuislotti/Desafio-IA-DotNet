using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Infrastructure.Search;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace CustomerPlatform.IntegrationTests.Tests.Infrastructure.Search
{
    [TestFixture]
    [RequiresThread]
    [SetCulture("pt-BR")]
    [Category("Infrastructure")]
    public sealed class ElasticSearchIntegrationTests
    {
        #region Variables
        private ElasticCustomerSearchService _searchService = null!;
        private ElasticSearchInitializer _initializer = null!;
        private ElasticSearchOptions _options = null!;
        #endregion

        #region SetUp Methods
        [SetUp]
        public async Task SetUp()
        {
            _options = new ElasticSearchOptions
            {
                ConnectionString = GlobalSetup.ElasticSearchUrl,
                IndexName = $"customers-tests-{Guid.NewGuid():N}"
            };

            _initializer = new ElasticSearchInitializer(Options.Create(_options));
            _searchService = new ElasticCustomerSearchService(Options.Create(_options));

            await _initializer.EnsureIndexAsync().ConfigureAwait(false);
        }
        #endregion

        #region Test Methods - IndexAndSearch Valid Cases
        [Test]
        public async Task IndexAndSearch_ShouldReturnCustomer()
        {
            // Arranjo
            var customerId = Guid.NewGuid();
            var customer = new CustomerDto
            {
                Id = customerId,
                CustomerType = TipoCliente.PF,
                Document = "52998224725",
                Name = "Cliente Teste",
                Email = "cliente@teste.com",
                Phone = "11999999999",
                Address = new AddressDto
                {
                    Street = "Rua Teste",
                    Number = "123",
                    Complement = "Apto 10",
                    PostalCode = "12345000",
                    City = "Sao Paulo",
                    State = "SP"
                },
                CreatedAt = DateTime.UtcNow
            };

            var criteria = new CustomerSearchCriteria
            {
                Name = customer.Name,
                PageNumber = 1,
                PageSize = 10
            };

            // Acao
            var indexResult = await _searchService.IndexAsync(customer).ConfigureAwait(false);
            var searchResult = await _searchService.SearchAsync(criteria).ConfigureAwait(false);

            // Assertiva
            Assert.That(indexResult.IsSuccess, Is.True);
            Assert.That(searchResult.IsSuccess, Is.True);
            Assert.That(searchResult.Data, Is.Not.Null);
            Assert.That(searchResult.Data!, Has.Some.Matches<CustomerDto>(current => current.Id == customerId));
        }
        #endregion
    }
}
