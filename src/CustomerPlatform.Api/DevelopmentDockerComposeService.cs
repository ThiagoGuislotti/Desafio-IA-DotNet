using CustomerPlatform.Infrastructure.FluentDocker.Builders;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace CustomerPlatform.Api
{
    /// <summary>
    /// Servico que inicia o docker compose em ambiente de desenvolvimento.
    /// </summary>
    public sealed class DevelopmentDockerComposeService : IHostedService
    {
        #region Public Properties
        /// <summary>
        /// Indica se o docker compose foi iniciado.
        /// </summary>
        public static bool IsDockerComposeRunning { get; private set; }
        #endregion

        #region Variables
        private ICompositeService? _compositeService;
        private readonly IHostEnvironment _environment;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger<DevelopmentDockerComposeService> _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="environment">Ambiente da aplicacao.</param>
        /// <param name="lifetime">Ciclo de vida da aplicacao.</param>
        /// <param name="logger">Logger do servico.</param>
        public DevelopmentDockerComposeService(
            IHostEnvironment environment,
            IHostApplicationLifetime lifetime,
            ILogger<DevelopmentDockerComposeService> logger)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!_environment.IsDevelopment())
                    return Task.CompletedTask;

                _logger.LogInformation(
                    "[Starting: DockerCompose]-[{Environment}] - [{Application}]",
                    _environment.EnvironmentName,
                    AppDomain.CurrentDomain.FriendlyName);

                var dockerRoot = GetDockerRoot();
                var serviceName = "customerplatform-development";

                _compositeService = new Builder()
                    .UseContainer()
                    .UseCompose()
                    .ServiceName(serviceName)
                    .FromFile(Path.Combine(dockerRoot, "docker-compose-postgres.yaml"))
                    .FromFile(Path.Combine(dockerRoot, "docker-compose-rabbitmq.yaml"))
                    .FromFile(Path.Combine(dockerRoot, "docker-compose-elasticsearch.yaml"))
                    .FromFile(Path.Combine(dockerRoot, "docker-compose-kibana.yaml"))
                    .FromFile(Path.Combine(dockerRoot, "docker-compose-otel.yaml"))
                    .FromFile(Path.Combine(dockerRoot, "docker-compose-aspire-dashboard.yaml"))
                    .WithResolvedEnvironment(Path.Combine(dockerRoot, ".env"))
                    .WaitForHealthy(30000)
                    .RemoveOrphans()
                    .Build()
                    .Start();

                _lifetime.ApplicationStopping.Register(OnShutdown);
                IsDockerComposeRunning = true;

                _logger.LogInformation(
                    "[Started: DockerCompose]-[{Environment}] - [{Application}]",
                    _environment.EnvironmentName,
                    AppDomain.CurrentDomain.FriendlyName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[Error: DockerCompose]-[{Environment}] - [{Application}]",
                    _environment.EnvironmentName,
                    AppDomain.CurrentDomain.FriendlyName);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Private Methods/Operators
        private string GetDockerRoot()
        {
            return Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "..", "..", "docker"));
        }

        private void OnShutdown()
        {
            try
            {
                if (_compositeService is null || _compositeService.State != ServiceRunningState.Running)
                    return;

                _logger.LogInformation(
                    "[Stopping: DockerCompose]-[{Environment}] - [{Application}]",
                    _environment.EnvironmentName,
                    AppDomain.CurrentDomain.FriendlyName);

                _compositeService.Dispose();

                _logger.LogInformation(
                    "[Stopped: DockerCompose]-[{Environment}] - [{Application}]",
                    _environment.EnvironmentName,
                    AppDomain.CurrentDomain.FriendlyName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[Error: DockerCompose]-[{Environment}] - [{Application}]",
                    _environment.EnvironmentName,
                    AppDomain.CurrentDomain.FriendlyName);
            }
        }
        #endregion
    }
}