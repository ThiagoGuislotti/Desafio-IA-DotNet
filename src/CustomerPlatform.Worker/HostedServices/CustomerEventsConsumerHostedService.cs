using CustomerPlatform.Domain.Events;
using CustomerPlatform.Infrastructure.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomerPlatform.Worker.HostedServices
{
    /// <summary>
    /// Consome eventos de cliente e executa indexacao e deduplicacao.
    /// </summary>
    public sealed class CustomerEventsConsumerHostedService : BackgroundService
    {
        #region Variables
        private readonly RabbitMqEventConsumer _consumer;
        private readonly RabbitMqOptions _options;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CustomerEventsConsumerHostedService> _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="consumer">Consumidor RabbitMQ.</param>
        /// <param name="options">Opcoes do RabbitMQ.</param>
        /// <param name="scopeFactory">Factory de escopo.</param>
        /// <param name="logger">Logger da execucao.</param>
        public CustomerEventsConsumerHostedService(
            RabbitMqEventConsumer consumer,
            IOptions<RabbitMqOptions> options,
            IServiceScopeFactory scopeFactory,
            ILogger<CustomerEventsConsumerHostedService> logger)
        {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Protected Methods/Operators
        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer
                .StartAsync(_options.QueueName, HandleMessageAsync, stoppingToken)
                .ConfigureAwait(false);

            await Task.Delay(Timeout.Infinite, stoppingToken).ConfigureAwait(false);
        }
        #endregion

        #region Private Methods/Operators
        private async Task HandleMessageAsync(EventMessage message, CancellationToken cancellationToken)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            if (message.EventType != ClienteCriado.EventTypeName &&
                message.EventType != ClienteAtualizado.EventTypeName)
                return;

            using var scope = _scopeFactory.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<CustomerEventDispatcher>();

            var result = await dispatcher.HandleAsync(message, cancellationToken).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "Falha ao processar evento {EventId}: {Message}",
                    message.EventId,
                    result.Message);
                throw new InvalidOperationException(result.Message ?? "Falha ao processar evento.");
            }
        }
        #endregion
    }
}
