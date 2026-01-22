using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.DependencyInjections;
using CustomerPlatform.Infrastructure.Messaging;
using CustomerPlatform.Infrastructure.Observability;
using CustomerPlatform.Infrastructure.Search;
using CustomerPlatform.IntegrationTests.Assets;
using CustomerPlatform.Worker.DependencyInjections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomerPlatform.IntegrationTests.Tests.EndToEnd
{
    /// <summary>
    /// Testes ponta a ponta do fluxo completo.
    /// </summary>
    [TestFixture]
    [RequiresThread]
    [SetCulture("pt-BR")]
    [Category("EndToEnd")]
    public sealed class CustomerFlowIntegrationTests
    {
        #region Nested types
        private sealed class ApiApplicationFactory : WebApplicationFactory<Program>
        {
            #region Protected Methods/Operators
            protected override void ConfigureWebHost(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Test");
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    var testConfiguration = new ConfigurationBuilder()
                        .SetBasePath(AppContext.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                        .AddEnvironmentVariables()
                        .Build();

                    config.AddConfiguration(testConfiguration);
                });
            }
            #endregion
        }
        #endregion

        #region Variables
        private ConfigureServices _configureServices = null!;
        private IServiceScope _scope = null!;
        private CustomerPlatformDbContext _dbContext = null!;
        private ApiApplicationFactory _factory = null!;
        private HttpClient _client = null!;
        private IHost? _workerHost;
        private string _indexName = string.Empty;
        private string _queueName = string.Empty;
        private JsonSerializerOptions _jsonOptions = null!;
        #endregion

        #region SetUp Methods
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            SetEnvironmentVariable("ConnectionStrings__PostgreSql", configuration.GetConnectionString("PostgreSql"));
            SetEnvironmentVariable("ConnectionStrings__RabbitMq", configuration.GetConnectionString("RabbitMq"));
            SetEnvironmentVariable("ConnectionStrings__ElasticSearch", configuration.GetConnectionString("ElasticSearch"));
        }

        [SetUp]
        public async Task SetUp()
        {
            _indexName = $"customers-e2e-{Guid.NewGuid():N}";
            _queueName = $"customerplatform-e2e-{Guid.NewGuid():N}";
            Environment.SetEnvironmentVariable("ElasticSearch__IndexName", _indexName);
            Environment.SetEnvironmentVariable("RabbitMq__QueueName", _queueName);

            _configureServices = new ConfigureServices();
            _scope = _configureServices.ServiceProvider.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<CustomerPlatformDbContext>();

            var initializer = _scope.ServiceProvider.GetRequiredService<ElasticSearchInitializer>();
            await initializer.EnsureIndexAsync().ConfigureAwait(false);
            await ClearDatabaseAsync().ConfigureAwait(false);

            _factory = new ApiApplicationFactory();
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_workerHost is not null)
            {
                await _workerHost.StopAsync().ConfigureAwait(false);
                _workerHost.Dispose();
                _workerHost = null;
            }

            _client.Dispose();
            _factory.Dispose();
            _scope.Dispose();
            Environment.SetEnvironmentVariable("ElasticSearch__IndexName", null);
            Environment.SetEnvironmentVariable("RabbitMq__QueueName", null);
        }
        #endregion

        #region Test Methods - EndToEndFlow Valid Cases
        [Test]
        public async Task CriarCliente_DevePublicarConsumirEBuscar()
        {
            // Arranjo
            var command = new CreateIndividualCustomerCommand
            {
                FullName = "Cliente E2E",
                Cpf = "02453141210",
                Email = "e2e@teste.com",
                Phone = "11988887777",
                BirthDate = new DateOnly(1990, 1, 10),
                Address = new AddressDto
                {
                    Street = "Rua E2E",
                    Number = "100",
                    Complement = "Apto 12",
                    PostalCode = "12345000",
                    City = "Sao Paulo",
                    State = "SP"
                }
            };

            // Acao
            var response = await _client.PostAsJsonAsync("/customers/pf", command, _jsonOptions).ConfigureAwait(false);
            var created = await response.Content.ReadFromJsonAsync<CustomerDto>(_jsonOptions).ConfigureAwait(false);

            // Assertiva
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(created, Is.Not.Null);

            var pending = await WaitForOutboxAsync(created!.Id, TimeSpan.FromSeconds(10)).ConfigureAwait(false);
            Assert.That(pending, Is.Not.Null);
            Assert.That(pending!.ProcessedAt, Is.Null);

            _workerHost = CreateWorkerHost(_configureServices.Configuration);
            await _workerHost.StartAsync().ConfigureAwait(false);

            var processed = await WaitForOutboxProcessedAsync(created.Id, TimeSpan.FromSeconds(30)).ConfigureAwait(false);
            Assert.That(processed, Is.Not.Null);
            Assert.That(processed!.ProcessedAt, Is.Not.Null);

            var found = await WaitForApiSearchAsync(command.FullName, created.Id, TimeSpan.FromSeconds(30))
                .ConfigureAwait(false);
            Assert.That(found, Is.True);
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

        private async Task<CustomerPlatform.Infrastructure.Data.Context.Entities.OutboxEvent?> WaitForOutboxAsync(
            Guid customerId,
            TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            var cancellationToken = cts.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                var outboxEvents = await _dbContext.OutboxEvents
                    .AsNoTracking()
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                var match = outboxEvents.FirstOrDefault(current => MatchesCustomer(current, customerId));
                if (match is not null)
                    return match;

                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        private async Task<CustomerPlatform.Infrastructure.Data.Context.Entities.OutboxEvent?> WaitForOutboxProcessedAsync(
            Guid customerId,
            TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            var cancellationToken = cts.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                var outboxEvents = await _dbContext.OutboxEvents
                    .AsNoTracking()
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                var match = outboxEvents.FirstOrDefault(current =>
                    current.ProcessedAt != null && MatchesCustomer(current, customerId));

                if (match is not null)
                    return match;

                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        private static bool MatchesCustomer(
            CustomerPlatform.Infrastructure.Data.Context.Entities.OutboxEvent outboxEvent,
            Guid customerId)
        {
            if (outboxEvent is null)
                return false;

            try
            {
                var payload = JsonSerializer.Deserialize<EventMessage>(
                    outboxEvent.Payload,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return payload?.CustomerId == customerId;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> WaitForApiSearchAsync(string name, Guid customerId, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);
            var cancellationToken = cts.Token;

            while (!cancellationToken.IsCancellationRequested)
            {
                var response = await _client
                    .GetAsync($"/customers/search?name={Uri.EscapeDataString(name)}", cancellationToken)
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var customers = await response.Content
                        .ReadFromJsonAsync<List<CustomerDto>>(_jsonOptions, cancellationToken)
                        .ConfigureAwait(false);

                    if (customers?.Any(current => current.Id == customerId) == true)
                        return true;
                }

                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
            }

            return false;
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

        private static void SetEnvironmentVariable(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                return;

            Environment.SetEnvironmentVariable(key, value, EnvironmentVariableTarget.Process);
        }
        #endregion
    }
}