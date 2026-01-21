using CustomerPlatform.IntegrationTests.Assets;
using CustomerPlatform.Infrastructure.Data.Context;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace CustomerPlatform.IntegrationTests.Tests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        #region Variables
        private static readonly Regex VariablePattern = new Regex(@"\$\{(?<key>[^}]+)\}", RegexOptions.Compiled);
        private ICompositeService? _compositeService;
        private static string? _postgresConnectionString;
        private static string? _rabbitMqConnectionString;
        private static string? _elasticSearchUrl;
        #endregion

        #region Public Properties
        /// <summary>
        /// Connection string do PostgreSQL.
        /// </summary>
        public static string PostgresConnectionString =>
            _postgresConnectionString ?? throw new InvalidOperationException("Ambiente de testes nao inicializado.");

        /// <summary>
        /// Connection string do RabbitMQ.
        /// </summary>
        public static string RabbitMqConnectionString =>
            _rabbitMqConnectionString ?? throw new InvalidOperationException("Ambiente de testes nao inicializado.");

        /// <summary>
        /// Url do ElasticSearch.
        /// </summary>
        public static string ElasticSearchUrl =>
            _elasticSearchUrl ?? throw new InvalidOperationException("Ambiente de testes nao inicializado.");
        #endregion

        #region SetUp Methods
        [OneTimeSetUp]
        public async Task RunBeforeAnyTestsAsync()
        {
            try
            {
                var root = LocateRepositoryRoot();
                var dockerRoot = Path.Combine(root, "docker");
                ApplyEnvironmentVariables(Path.Combine(dockerRoot, ".env"));

                _compositeService = new Builder()
                    .UseContainer()
                    .UseCompose()
                    .ServiceName("customerplatform-integration-tests")
                    .FromFile(Path.Combine(dockerRoot, "services", "databases", "docker-compose-postgres.yaml"))
                    .FromFile(Path.Combine(dockerRoot, "services", "messaging", "docker-compose-rabbitmq.yaml"))
                    .FromFile(Path.Combine(dockerRoot, "services", "search", "docker-compose-elasticsearch.yaml"))
                    .WaitForHealthy(60000)
                    .RemoveOrphans()
                    .Build()
                    .Start();

                LoadConnectionStrings();

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60));
                var dbTask = WaitForPostgresAsync(cancellationTokenSource.Token);
                var completed = await Task.WhenAny(dbTask, Task.Delay(Timeout.Infinite, cancellationTokenSource.Token)).ConfigureAwait(false);

                if (completed != dbTask)
                    Assert.Ignore("Test ignored due to failure to connect to services.");

                await Task.WhenAll(dbTask).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Assert.Ignore($"Error while configuring docker-compose: {ex.Message}");
            }
        }

        [OneTimeTearDown]
        public void RunAfterAllTests()
        {
            _compositeService?.Dispose();
        }
        #endregion

        #region Private Methods/Operators
        private static void ApplyEnvironmentVariables(string envFilePath)
        {
            if (!File.Exists(envFilePath))
                return;

            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var line in File.ReadAllLines(envFilePath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal))
                    continue;

                var separatorIndex = trimmed.IndexOf('=');
                if (separatorIndex <= 0)
                    continue;

                var key = trimmed[..separatorIndex].Trim();
                var rawValue = trimmed[(separatorIndex + 1)..].Trim().Trim('"');
                values[key] = rawValue;
            }

            foreach (var pair in values)
                System.Environment.SetEnvironmentVariable(pair.Key, ResolveVariables(pair.Value, values));
        }

        private static void LoadConnectionStrings()
        {
            var configureServices = new ConfigureServices();
            var configuration = configureServices.Configuration;

            _postgresConnectionString = GetRequiredConnectionString(configuration, "PostgreSql");
            _rabbitMqConnectionString = GetRequiredConnectionString(configuration, "RabbitMq");
            _elasticSearchUrl = GetRequiredConnectionString(configuration, "ElasticSearch");
        }

        private static string GetRequiredConnectionString(IConfiguration configuration, string name)
        {
            var value = configuration.GetConnectionString(name);
            if (!string.IsNullOrWhiteSpace(value))
                return value;

            throw new InvalidOperationException($"Connection string obrigatoria ausente: {name}.");
        }

        private static string ResolveVariables(string value, IDictionary<string, string> values)
        {
            return VariablePattern.Replace(value, match =>
            {
                var key = match.Groups["key"].Value;
                return values.TryGetValue(key, out var replacement) ? replacement : match.Value;
            });
        }

        private static string LocateRepositoryRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);

            while (current is not null)
            {
                var solutionPath = Path.Combine(current.FullName, "CustomerPlatform.sln");
                if (File.Exists(solutionPath))
                    return current.FullName;

                current = current.Parent;
            }

            throw new DirectoryNotFoundException("Nao foi possivel localizar a raiz do repositorio.");
        }

        private static async Task WaitForPostgresAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var configureServices = new ConfigureServices();
                    using var scope = configureServices.ServiceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<CustomerPlatformDbContext>();
                    
                    await dbContext.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
                    await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

                    if (await dbContext.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false))
                        return;
                }
                catch
                {
                }

                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            }
        }
        #endregion
    }
}
