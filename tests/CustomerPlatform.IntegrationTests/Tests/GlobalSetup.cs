using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.IntegrationTests.Assets;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CustomerPlatform.IntegrationTests.Tests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        #region Variables
        private ICompositeService? _compositeService;
        private static TestEnvironment? _environment;
        #endregion

        #region Public Properties
        /// <summary>
        /// Ambiente carregado para testes.
        /// </summary>
        public static TestEnvironment Environment =>
            _environment ?? throw new InvalidOperationException("Ambiente de testes nao inicializado.");
        #endregion

        #region SetUp Methods
        [OneTimeSetUp]
        public async Task RunBeforeAnyTestsAsync()
        {
            try
            {
                var root = RepositoryLocator.LocateRepositoryRoot();
                var dockerRoot = Path.Combine(root, "docker");
                var envFile = Path.Combine(dockerRoot, ".env");
                var environmentVariables = EnvFileLoader.Load(envFile);

                foreach (var variable in environmentVariables)
                    System.Environment.SetEnvironmentVariable(variable.Key, variable.Value);

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

                _environment = TestEnvironment.Load();

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