using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.DependencyInjections;
using CustomerPlatform.Infrastructure.Observability;
using CustomerPlatform.Infrastructure.Search;
using CustomerPlatform.IntegrationTests.Assets;
using CustomerPlatform.Worker.DependencyInjections;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CustomerPlatform.IntegrationTests.Tests.Worker
{
    [TestFixture]
    [RequiresThread]
    [SetCulture("pt-BR")]
    [Category("Worker")]
    public sealed class WorkerFlowIntegrationTests
    {
        #region Variables
        private ConfigureServices _configureServices = null!;
        private CustomerPlatformDbContext _dbContext = null!;
        private IHost _workerHost = null!;
        private string _indexName = string.Empty;
        private string _queueName = string.Empty;
        #endregion

        #region SetUp Methods
        [SetUp]
        public async Task SetUp()
        {
            _indexName = $"customers-worker-{Guid.NewGuid():N}";
            _queueName = $"customerplatform-worker-{Guid.NewGuid():N}";
            Environment.SetEnvironmentVariable("ElasticSearch__IndexName", _indexName);
            Environment.SetEnvironmentVariable("RabbitMq__QueueName", _queueName);

            _configureServices = new ConfigureServices();
            _dbContext = _configureServices.ServiceProvider.GetRequiredService<CustomerPlatformDbContext>();

            var initializer = _configureServices.ServiceProvider.GetRequiredService<ElasticSearchInitializer>();
            await initializer.EnsureIndexAsync().ConfigureAwait(false);
            await ClearDatabaseAsync().ConfigureAwait(false);

            _workerHost = CreateWorkerHost(_configureServices.Configuration);
            await _workerHost.StartAsync().ConfigureAwait(false);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_workerHost is not null)
                await _workerHost.StopAsync().ConfigureAwait(false);

            _workerHost?.Dispose();
            await ClearDatabaseAsync().ConfigureAwait(false);
            _configureServices.ServiceProvider.Dispose();
            Environment.SetEnvironmentVariable("ElasticSearch__IndexName", null);
            Environment.SetEnvironmentVariable("RabbitMq__QueueName", null);
        }
        #endregion

        #region Test Methods - OutboxFlow Valid Cases
        [Test]
        public async Task FluxoOutbox_DeveIndexarCliente()
        {
            // Arrange
            var mediator = _configureServices.ServiceProvider.GetRequiredService<IMediator>();
            var searchService = _configureServices.ServiceProvider.GetRequiredService<ICustomerSearchService>();

            var command = new CreateIndividualCustomerCommand
            {
                FullName = "Cliente Worker",
                Cpf = "65653318124",
                Email = "worker@teste.com",
                Phone = "11999999999",
                BirthDate = new DateOnly(1990, 1, 1),
                Address = new AddressDto
                {
                    Street = "Rua Worker",
                    Number = "100",
                    Complement = "Apto 1",
                    PostalCode = "12345000",
                    City = "Sao Paulo",
                    State = "SP"
                }
            };

            // Act
            var createResult = await mediator.Send(command).ConfigureAwait(false);
            // Aguarda o worker processar a outbox e indexar no Elastic.
            var indexedResult = await WaitForIndexedCustomerAsync(
                    searchService,
                    createResult.Data?.Id ?? Guid.Empty,
                    TimeSpan.FromSeconds(30))
                .ConfigureAwait(false);

            // Assert
            Assert.That(createResult.IsSuccess, Is.True, createResult.Message);
            Assert.That(createResult.Data, Is.Not.Null);
            Assert.That(indexedResult.IsSuccess, Is.True, indexedResult.Message);
            Assert.That(indexedResult.Data, Is.Not.Null);
            Assert.That(indexedResult.Data!.Id, Is.EqualTo(createResult.Data!.Id));
        }
        #endregion

        #region Private Methods/Operators
        private static IHost CreateWorkerHost(IConfiguration configuration)
        {
            var builder = Host.CreateApplicationBuilder();
            builder.Configuration.Sources.Clear();
            builder.Configuration.AddConfiguration(configuration);

            builder.Services.AddCustomerPlatformObservability(builder.Configuration, "CustomerPlatform.Worker");
            builder.Services.AddCustomerPlatformInfrastructure(builder.Configuration);
            builder.Services.AddCustomerPlatformWorker(builder.Configuration);

            return builder.Build();
        }

        private static async Task<Result<CustomerDto>> WaitForIndexedCustomerAsync(
            ICustomerSearchService searchService,
            Guid customerId,
            TimeSpan timeout)
        {
            if (customerId == Guid.Empty)
                return Result<CustomerDto>.Failure("Id obrigatorio.");

            using var cancellationTokenSource = new CancellationTokenSource(timeout);
            var cancellationToken = cancellationTokenSource.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await searchService
                        .GetByIdAsync(customerId, cancellationToken)
                        .ConfigureAwait(false);

                    if (result.IsSuccess && result.Data is not null)
                        return result;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                }

                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }

            return Result<CustomerDto>.Failure("Timeout aguardando indexacao.");
        }

        private async Task ClearDatabaseAsync()
        {
            var suspicions = await _dbContext.DuplicateSuspicions.ToListAsync().ConfigureAwait(false);
            if (suspicions.Count > 0)
                _dbContext.DuplicateSuspicions.RemoveRange(suspicions);

            var outbox = await _dbContext.OutboxEvents.ToListAsync().ConfigureAwait(false);
            if (outbox.Count > 0)
                _dbContext.OutboxEvents.RemoveRange(outbox);

            var customers = await _dbContext.Customers.ToListAsync().ConfigureAwait(false);
            if (customers.Count > 0)
                _dbContext.Customers.RemoveRange(customers);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
        #endregion
    }
}