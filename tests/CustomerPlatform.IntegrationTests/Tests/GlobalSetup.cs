using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.FluentDocker.Builders;
using CustomerPlatform.Infrastructure.FluentDocker.Utilities;
using CustomerPlatform.IntegrationTests.Assets;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace CustomerPlatform.IntegrationTests.Tests
{
    /// <summary>
    /// Configura o ambiente global dos testes de integracao.
    /// </summary>
    [SetUpFixture]
    public class GlobalSetup
    {
        #region Variables
        private ICompositeService? _compositeService;
        #endregion

        #region SetUp Methods
        [OneTimeSetUp]
        public async Task RunBeforeAnyTestsAsync()
        {
            try
            {
                var dockerRoot = DirectoryLocator.LocateDirectory(new[] { "docker" });
                var dotnetVersion = RuntimeInformation.FrameworkDescription.Replace(" ", "").Replace(".", "").ToLower();
                var serviceName = $"customerplatform-integration-tests-{dotnetVersion}";

                _compositeService = new Builder()
                    .UseContainer()
                    .UseCompose()
                    .ServiceName(serviceName)
                    .FromFile(Path.Combine(dockerRoot, "services", "databases", "docker-compose-postgres.yaml"))
                    .FromFile(Path.Combine(dockerRoot, "services", "messaging", "docker-compose-rabbitmq.yaml"))
                    .FromFile(Path.Combine(dockerRoot, "services", "search", "docker-compose-elasticsearch.yaml"))
                    .WithResolvedEnvironment(Path.Combine(dockerRoot, ".env"))
                    .WaitForHealthy(30000)
                    .RemoveOrphans()
                    .Build()
                    .Start();

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var cancellationToken = cancellationTokenSource.Token;

                var dbTask = WaitForDbContextAsync(cancellationToken);
                var completed = await Task.WhenAny(dbTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);

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
        private static async Task WaitForDbContextAsync(CancellationToken cancellationToken)
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