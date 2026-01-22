using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Infrastructure.Search;
using CustomerPlatform.IntegrationTests.Assets;
using Microsoft.Extensions.Configuration;
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
            var configuration = new ConfigureServices().Configuration;
            _options = new ElasticSearchOptions
            {
                ConnectionString = configuration.GetConnectionString("ElasticSearch") ?? string.Empty,
                IndexName = $"customers-tests-{Guid.NewGuid():N}"
            };

            _initializer = new ElasticSearchInitializer(Options.Create(_options));
            _searchService = new ElasticCustomerSearchService(Options.Create(_options));

            await _initializer.EnsureIndexAsync().ConfigureAwait(false);
        }
        #endregion

        #region Test Methods - IndexAndSearch Valid Cases
        [Test]
        public async Task IndexarEBuscar_DeveRetornarCliente()
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

        #region Test Methods - SearchAsync Valid Cases
        [Test]
        public async Task SearchAsync_DeveOrdenarPorRelevancia()
        {
            // Arranjo
            var exactId = Guid.NewGuid();
            var similarId = Guid.NewGuid();
            var exact = CustomerSeed.CreateIndividual(
                exactId,
                "Carlos Alberto",
                "22233344455",
                "carlos@teste.com",
                "11999990000");

            var similar = CustomerSeed.CreateIndividual(
                similarId,
                "Carlos Al",
                "22233344456",
                "carlos2@teste.com",
                "11999991111");

            await _searchService.IndexAsync(exact).ConfigureAwait(false);
            await _searchService.IndexAsync(similar).ConfigureAwait(false);

            var criteria = new CustomerSearchCriteria
            {
                Name = "Carlos Alberto",
                PageNumber = 1,
                PageSize = 10
            };

            // Acao
            var searchResult = await _searchService.SearchAsync(criteria).ConfigureAwait(false);

            // Assertiva
            Assert.That(searchResult.IsSuccess, Is.True);
            Assert.That(searchResult.Data, Is.Not.Null);

            var results = searchResult.Data!.ToList();
            Assert.That(results.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(results[0].Id, Is.EqualTo(exactId));
        }

        [Test]
        public async Task SearchAsync_SeedGrande_DeveRetornarExatoPrimeiro()
        {
            // Arranjo
            var seed = CustomerSeed.CreateIndividualsBatch(100, "Seed Cliente");
            foreach (var customer in seed)
                await _searchService.IndexAsync(customer).ConfigureAwait(false);

            var exactId = Guid.NewGuid();
            var similarId = Guid.NewGuid();
            var exact = CustomerSeed.CreateIndividual(
                exactId,
                "Cliente Relevante",
                "33344455566",
                "relevante@teste.com",
                "11911112222");

            var similar = CustomerSeed.CreateIndividual(
                similarId,
                "Cliente Relev",
                "33344455567",
                "relevante2@teste.com",
                "11911113333");

            await _searchService.IndexAsync(exact).ConfigureAwait(false);
            await _searchService.IndexAsync(similar).ConfigureAwait(false);

            var criteria = new CustomerSearchCriteria
            {
                Name = "Cliente Relevante",
                PageNumber = 1,
                PageSize = 5
            };

            // Acao
            var searchResult = await _searchService.SearchAsync(criteria).ConfigureAwait(false);

            // Assertiva
            Assert.That(searchResult.IsSuccess, Is.True);
            Assert.That(searchResult.Data, Is.Not.Null);

            var results = searchResult.Data!.ToList();
            Assert.That(results.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(results[0].Id, Is.EqualTo(exactId));
        }

        [Test]
        public async Task SearchAsync_FiltroPorEmailETelefone_DeveRetornarClienteUnico()
        {
            // Arranjo
            var targetId = Guid.NewGuid();
            var target = new CustomerDto
            {
                Id = targetId,
                CustomerType = TipoCliente.PF,
                Document = "98765432109",
                Name = "Cliente Filtro",
                Email = "filtro@teste.com",
                Phone = "11912345678",
                Address = new AddressDto
                {
                    Street = "Rua Teste",
                    Number = "50",
                    Complement = "Casa",
                    PostalCode = "12345000",
                    City = "Sao Paulo",
                    State = "SP"
                },
                CreatedAt = DateTime.UtcNow
            };

            var other = CustomerSeed.CreateIndividual(
                Guid.NewGuid(),
                "Cliente Outro",
                "98765432108",
                "outro@teste.com",
                "11900001111");

            await _searchService.IndexAsync(target).ConfigureAwait(false);
            await _searchService.IndexAsync(other).ConfigureAwait(false);

            var criteria = new CustomerSearchCriteria
            {
                Email = "filtro",
                Phone = "11912345678",
                PageNumber = 1,
                PageSize = 5
            };

            // Acao
            var searchResult = await _searchService.SearchAsync(criteria).ConfigureAwait(false);

            // Assertiva
            Assert.That(searchResult.IsSuccess, Is.True);
            Assert.That(searchResult.Data, Is.Not.Null);
            Assert.That(searchResult.Data!, Has.Exactly(1).Matches<CustomerDto>(current => current.Id == targetId));
        }
        #endregion
    }
}
