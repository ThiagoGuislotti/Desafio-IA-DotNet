using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.Deduplication;
using CustomerPlatform.Infrastructure.Messaging;
using CustomerPlatform.Infrastructure.Search;
using CustomerPlatform.IntegrationTests.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace CustomerPlatform.IntegrationTests.Tests.Infrastructure.Deduplication
{
    /// <summary>
    /// Testes de integracao para deduplicacao.
    /// </summary>
    [TestFixture]
    [RequiresThread]
    [SetCulture("pt-BR")]
    [Category("Deduplication")]
    public sealed class DeduplicationIntegrationTests
    {
        #region Variables
        private ConfigureServices _configureServices = null!;
        private IServiceScope _scope = null!;
        private CustomerPlatformDbContext _dbContext = null!;
        private DeduplicationProcessor _processor = null!;
        private ICustomerSearchService _searchService = null!;
        private RabbitMqOptions _rabbitOptions = null!;
        private decimal _threshold;
        private string _indexName = string.Empty;
        #endregion

        #region SetUp Methods
        [SetUp]
        public async Task SetUp()
        {
            _indexName = $"customers-dedup-{Guid.NewGuid():N}";
            Environment.SetEnvironmentVariable("ElasticSearch__IndexName", _indexName);

            _configureServices = new ConfigureServices();
            _scope = _configureServices.ServiceProvider.CreateScope();

            _dbContext = _scope.ServiceProvider.GetRequiredService<CustomerPlatformDbContext>();
            _processor = _scope.ServiceProvider.GetRequiredService<DeduplicationProcessor>();
            _searchService = _scope.ServiceProvider.GetRequiredService<ICustomerSearchService>();
            _rabbitOptions = _scope.ServiceProvider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            _threshold = _scope.ServiceProvider.GetRequiredService<IOptions<DeduplicationOptions>>().Value.Threshold;

            var initializer = _scope.ServiceProvider.GetRequiredService<ElasticSearchInitializer>();
            await initializer.EnsureIndexAsync().ConfigureAwait(false);
            await ClearSuspicionsAsync().ConfigureAwait(false);
        }

        [TearDown]
        public async Task TearDown()
        {
            await ClearSuspicionsAsync().ConfigureAwait(false);
            _scope.Dispose();
            Environment.SetEnvironmentVariable("ElasticSearch__IndexName", null);
        }
        #endregion

        #region Test Methods - ProcessAsync Valid Cases
        [Test]
        public async Task ProcessAsync_NomeETelefoneIguais_DeveCriarSuspeita()
        {
            // Arranjo
            var candidateId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var candidate = CustomerSeed.CreateIndividual(
                candidateId,
                "Maria Souza",
                "12345678901",
                "maria1@teste.com",
                "11999999999");

            var source = CustomerSeed.CreateIndividual(
                sourceId,
                "Maria Souza",
                "12345678902",
                "maria2@teste.com",
                "11999999999");

            await _searchService.IndexAsync(candidate).ConfigureAwait(false);

            using var consumer = new RabbitMqEventConsumer(Options.Create(_rabbitOptions));
            var queueName = $"customerplatform-dedup-{Guid.NewGuid():N}";
            var tcs = new TaskCompletionSource<EventMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await consumer.StartAsync(queueName, (message, _) =>
            {
                if (message.EventType == DuplicataSuspeitaEvent.EventTypeName)
                    tcs.TrySetResult(message);

                return Task.CompletedTask;
            }, cts.Token).ConfigureAwait(false);

            // Acao
            var result = await _processor.ProcessAsync(source, cts.Token).ConfigureAwait(false);
            var suspicion = await GetSuspicionAsync(sourceId, candidateId).ConfigureAwait(false);
            var completed = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(10), cts.Token))
                .ConfigureAwait(false);

            // Assertiva
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(suspicion, Is.Not.Null);
            Assert.That(suspicion!.Score, Is.GreaterThanOrEqualTo(_threshold));
            Assert.That(completed, Is.EqualTo(tcs.Task));

            var received = await tcs.Task.ConfigureAwait(false);
            Assert.That(received.CustomerId, Is.EqualTo(sourceId));
            Assert.That(received.CandidateCustomerId, Is.EqualTo(candidateId));
        }

        [Test]
        public async Task ProcessAsync_NomeEEmailIguais_DeveCriarSuspeita()
        {
            // Arranjo
            var candidateId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var candidate = CustomerSeed.CreateIndividual(
                candidateId,
                "Joao da Silva",
                "98765432100",
                "joao@teste.com",
                "11911112222");

            var source = CustomerSeed.CreateIndividual(
                sourceId,
                "Joao Silva",
                "98765432101",
                "joao@teste.com",
                "11933334444");

            await _searchService.IndexAsync(candidate).ConfigureAwait(false);

            // Acao
            var result = await _processor.ProcessAsync(source).ConfigureAwait(false);
            var suspicion = await GetSuspicionAsync(sourceId, candidateId).ConfigureAwait(false);

            // Assertiva
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(suspicion, Is.Not.Null);
            Assert.That(suspicion!.Score, Is.GreaterThanOrEqualTo(_threshold));
        }

        [Test]
        public async Task ProcessAsync_SeedGrande_DeveCriarSuspeitaUnica()
        {
            // Arranjo
            var seed = CustomerSeed.CreateIndividualsBatch(100, "Seed Cliente");
            foreach (var customer in seed)
                await _searchService.IndexAsync(customer).ConfigureAwait(false);

            var candidateId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var candidate = CustomerSeed.CreateIndividual(
                candidateId,
                "Cliente Alvo",
                "44556677889",
                "alvo@teste.com",
                "11977778888");

            var source = CustomerSeed.CreateIndividual(
                sourceId,
                "Cliente Alvo",
                "44556677880",
                "alvo2@teste.com",
                "11977778888");

            await _searchService.IndexAsync(candidate).ConfigureAwait(false);

            // Acao
            var result = await _processor.ProcessAsync(source).ConfigureAwait(false);
            var suspicions = await GetSuspicionsAsync(sourceId).ConfigureAwait(false);

            // Assertiva
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(suspicions, Is.Not.Null);
            Assert.That(suspicions!.Count, Is.EqualTo(1));
            Assert.That(suspicions[0].CandidateCustomerId, Is.EqualTo(candidateId));
            Assert.That(suspicions[0].Score, Is.GreaterThanOrEqualTo(_threshold));
        }
        #endregion

        #region Test Methods - ProcessAsync Invalid Cases
        [Test]
        public async Task ProcessAsync_NomeApenasSimilar_NaoDeveCriarSuspeita()
        {
            // Arranjo
            var candidateId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var candidate = CustomerSeed.CreateIndividual(
                candidateId,
                "Ana Pereira",
                "11122233344",
                "ana@teste.com",
                "11955556666");

            var source = CustomerSeed.CreateIndividual(
                sourceId,
                "Ana Pereira",
                "55566677788",
                "ana.outro@teste.com",
                "11977778888");

            await _searchService.IndexAsync(candidate).ConfigureAwait(false);

            // Acao
            var result = await _processor.ProcessAsync(source).ConfigureAwait(false);
            var suspicion = await GetSuspicionAsync(sourceId, candidateId).ConfigureAwait(false);

            // Assertiva
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(suspicion, Is.Null);
        }

        [Test]
        public async Task ProcessAsync_EmpresaEPessoaFisicaComNomesSimilares_NaoDeveCriarSuspeita()
        {
            // Arranjo
            var candidateId = Guid.NewGuid();
            var sourceId = Guid.NewGuid();
            var candidate = CustomerSeed.CreateCompany(
                candidateId,
                "Empresa Alfa",
                "Empresa Alfa",
                "45678912300",
                "contato@empresa.com",
                "1133334444");

            var source = CustomerSeed.CreateIndividual(
                sourceId,
                "Empresa Alfa",
                "99988877766",
                "cliente@teste.com",
                "1199990000");

            await _searchService.IndexAsync(candidate).ConfigureAwait(false);

            // Acao
            var result = await _processor.ProcessAsync(source).ConfigureAwait(false);
            var suspicion = await GetSuspicionAsync(sourceId, candidateId).ConfigureAwait(false);

            // Assertiva
            Assert.That(result.IsSuccess, Is.True, result.Message);
            Assert.That(suspicion, Is.Null);
        }
        #endregion

        #region Private Methods/Operators
        private async Task<CustomerPlatform.Infrastructure.Data.Context.Entities.DuplicateSuspicion?> GetSuspicionAsync(
            Guid customerId,
            Guid candidateCustomerId)
        {
            return await _dbContext.DuplicateSuspicions
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    current => current.CustomerId == customerId &&
                               current.CandidateCustomerId == candidateCustomerId)
                .ConfigureAwait(false);
        }

        private async Task<IReadOnlyList<CustomerPlatform.Infrastructure.Data.Context.Entities.DuplicateSuspicion>> GetSuspicionsAsync(
            Guid customerId)
        {
            return await _dbContext.DuplicateSuspicions
                .AsNoTracking()
                .Where(current => current.CustomerId == customerId)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        private async Task ClearSuspicionsAsync()
        {
            var suspicions = await _dbContext.DuplicateSuspicions.ToListAsync().ConfigureAwait(false);
            if (suspicions.Count == 0)
                return;

            _dbContext.DuplicateSuspicions.RemoveRange(suspicions);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion
    }
}
