using CustomerPlatform.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CustomerPlatform.Api
{
    /// <summary>
    /// Inicializa servicos essenciais em ambiente de desenvolvimento.
    /// </summary>
    public sealed class DevelopmentServiceInitializer : IHostedService
    {
        #region Variables
        private readonly IHostEnvironment _environment;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DevelopmentServiceInitializer> _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="environment">Ambiente da aplicacao.</param>
        /// <param name="logger">Logger do servico.</param>
        /// <param name="serviceProvider">Service provider.</param>
        public DevelopmentServiceInitializer(
            IHostEnvironment environment,
            ILogger<DevelopmentServiceInitializer> logger,
            IServiceProvider serviceProvider)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (_environment.IsDevelopment() && !DevelopmentDockerComposeService.IsDockerComposeRunning)
                    return;

                _logger.LogInformation(
                    "[Starting: Services]-[{Environment}] - [{Application}]",
                    _environment.EnvironmentName,
                    AppDomain.CurrentDomain.FriendlyName);

                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                var token = timeout.Token;

                var dbTask = WaitForDbContextAsync(token);
                var completed = await Task.WhenAny(dbTask).ConfigureAwait(false);
                await Task.WhenAll(completed).ConfigureAwait(false);

                _logger.LogInformation(
                    "[Started: Services]-[{Environment}] - [{Application}]",
                    _environment.EnvironmentName,
                    AppDomain.CurrentDomain.FriendlyName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "[Error: Services]-[{Environment}] - [{Application}]",
                    _environment.EnvironmentName,
                    AppDomain.CurrentDomain.FriendlyName);
            }
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Private Methods/Operators
        private async Task WaitForDbContextAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<CustomerPlatformDbContext>();
                    await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);

                    if (await dbContext.Database.CanConnectAsync(cancellationToken).ConfigureAwait(false))
                        return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "[Warning: Start DbContext]-[{Environment}] - [{Application}]",
                        _environment.EnvironmentName,
                        AppDomain.CurrentDomain.FriendlyName);
                }

                await Task.Delay(500, cancellationToken).ConfigureAwait(false);
            }
        }
        #endregion
    }
}