using CustomerPlatform.Domain.Entities;
using CustomerPlatform.Domain.ValueObjects;
using CustomerPlatform.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CustomerPlatform.IntegrationTests.Tests.Infrastructure.Data
{
    [TestFixture]
    [RequiresThread]
    [SetCulture("pt-BR")]
    [Category("Infrastructure")]
    public sealed class PostgresIntegrationTests
    {
        #region Variables
        private CustomerPlatformDbContext _dbContext = null!;
        #endregion

        #region SetUp Methods
        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<CustomerPlatformDbContext>()
                .UseNpgsql(GlobalSetup.PostgresConnectionString)
                .Options;

            _dbContext = new CustomerPlatformDbContext(options);
            await _dbContext.Database.MigrateAsync().ConfigureAwait(false);
        }

        [TearDown]
        public async Task TearDown()
        {
            var items = await _dbContext.Customers.ToListAsync().ConfigureAwait(false);
            if (items.Count > 0)
            {
                _dbContext.Customers.RemoveRange(items);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        #endregion

        #region Test Methods - PersistCustomer Valid Cases
        [Test]
        public async Task PersistCustomer_ShouldSaveAndLoad()
        {
            // Arranjo
            var address = new Endereco(
                "Rua Teste",
                "123",
                "Apto 10",
                "12345000",
                "Sao Paulo",
                "SP");

            var customer = ClientePessoaFisica.Criar(
                "Joao da Silva",
                "52998224725",
                "joao@test.com",
                "11999999999",
                new DateOnly(1990, 1, 1),
                address);

            // Acao
            _dbContext.Customers.Add(customer);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            var loaded = await _dbContext.Customers
                .FirstOrDefaultAsync(current => current.Id == customer.Id)
                .ConfigureAwait(false);

            // Assertiva
            Assert.That(loaded, Is.Not.Null);
            Assert.That(loaded!.Id, Is.EqualTo(customer.Id));
        }
        #endregion
    }
}
