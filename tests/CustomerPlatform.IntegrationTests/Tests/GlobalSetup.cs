using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace CustomerPlatform.IntegrationTests.Tests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        #region Variables
        private ICompositeService? _compositeService;
        #endregion

        #region OneTimeSetup Methods
        [OneTimeSetUp]
        public async Task RunBeforeAnyTestsAsync()
        {
            try
            {
                //var currentDirectory = NetToolsKit.Docker.Utilities.DirectoryLocator.LocateDirectory(["samples", "docker"]);
                //var dotnetVersion = RuntimeInformation.FrameworkDescription.Replace(" ", "").Replace(".", "").ToLower();
                //var serviceName = $"net-sample-development-tests-{dotnetVersion}";

                //_compositeService = new Builder()
                //    .UseContainer()
                //    .UseCompose()
                //    .ServiceName(serviceName)
                //    .FromFile(Path.Combine(currentDirectory, "services/storage/docker-compose-minio.yaml"))
                //    .FromFile(Path.Combine(currentDirectory, "services/databases/docker-compose-postgres.yaml"))
                //    .FromFile(Path.Combine(currentDirectory, "services/messaging/docker-compose-rabbitmq.yaml"))
                //    .WithResolvedEnvironment(Path.Combine(currentDirectory, ".env"))
                //    .WaitForHealthy(30000)
                //    .RemoveOrphans()
                //    .Build()
                //    .Start();

                using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var cancellationToken = cancellationTokenSource.Token;

                // Wait for services to be ready
                var dbTask = WaitForDbContextAsync(cancellationToken);

                var completedDbTask = await Task.WhenAny(dbTask, Task.Delay(Timeout.Infinite, cancellationToken)).ConfigureAwait(false);

                if (completedDbTask != dbTask)
                    Assert.Ignore("Test ignored due to failure to connect to services.");

                await dbTask;
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

        #region Private Methods
        private static async Task WaitForDbContextAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    //var configureServices = new ConfigureServices();

                    //using var scope = configureServices.ServiceProvider.CreateScope();
                    //var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    //await dbContext.Database.EnsureDeletedAsync(cancellationToken).ConfigureAwait(false);
                    //await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

                    //if (await dbContext.Database.CanConnectAsync(cancellationToken))
                    //    return;
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