using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.Data.Context.Entities;
using CustomerPlatform.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using System.Text;

namespace CustomerPlatform.Worker.HostedServices
{
    /// <summary>
    /// Publica eventos pendentes da outbox para o RabbitMQ.
    /// </summary>
    public sealed class OutboxPublisherHostedService : BackgroundService
    {
        #region Constants
        private const int BatchSize = 50;
        private const int PollDelaySeconds = 5;
        private const int RetryCount = 3;
        private const int RetryDelayMilliseconds = 200;
        private const int MaxErrorLength = 2000;
        #endregion

        #region Variables
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly RabbitMqOptions _options;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly AsyncPolicy _publishPolicy;
        private readonly ILogger<OutboxPublisherHostedService> _logger;
        private bool _disposed;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="scopeFactory">Factory de escopo.</param>
        /// <param name="options">Opcoes do RabbitMQ.</param>
        /// <param name="logger">Logger da execucao.</param>
        public OutboxPublisherHostedService(
            IServiceScopeFactory scopeFactory,
            IOptions<RabbitMqOptions> options,
            ILogger<OutboxPublisherHostedService> logger)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrWhiteSpace(_options.ConnectionString))
                throw new InvalidOperationException("ConnectionString do RabbitMQ nao configurada.");

            var factory = new ConnectionFactory
            {
                Uri = new Uri(_options.ConnectionString),
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_options.ExchangeName, _options.ExchangeType, durable: true, autoDelete: false);

            _publishPolicy = CreateRetryPolicy();
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public override void Dispose()
        {
            if (_disposed)
                return;

            _channel.Dispose();
            _connection.Dispose();
            _disposed = true;
            base.Dispose();
        }
        #endregion

        #region Protected Methods/Operators
        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var processedAny = await ProcessBatchAsync(stoppingToken).ConfigureAwait(false);
                if (!processedAny)
                    await Task.Delay(TimeSpan.FromSeconds(PollDelaySeconds), stoppingToken).ConfigureAwait(false);
            }
        }
        #endregion

        #region Private Methods/Operators
        private async Task<bool> ProcessBatchAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CustomerPlatformDbContext>();

            var pending = await dbContext.OutboxEvents
                .Where(outbox => outbox.ProcessedAt == null)
                .OrderBy(outbox => outbox.CreatedAt)
                .Take(BatchSize)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (pending.Count == 0)
                return false;

            foreach (var outboxEvent in pending)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    await _publishPolicy
                        .ExecuteAsync(ct => PublishAsync(outboxEvent, ct), cancellationToken)
                        .ConfigureAwait(false);

                    outboxEvent.MarkProcessed();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha ao publicar evento {EventId}", outboxEvent.EventId);
                    outboxEvent.RegisterFailure(NormalizeError(ex.Message));
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }

        private Task PublishAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken)
        {
            if (outboxEvent is null)
                throw new ArgumentNullException(nameof(outboxEvent));

            cancellationToken.ThrowIfCancellationRequested();

            var body = Encoding.UTF8.GetBytes(outboxEvent.Payload);
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = outboxEvent.EventId.ToString();
            properties.Type = outboxEvent.EventType;
            properties.Timestamp = new AmqpTimestamp(new DateTimeOffset(outboxEvent.OccurredAt).ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: _options.ExchangeName,
                routingKey: outboxEvent.EventType,
                basicProperties: properties,
                body: body);

            return Task.CompletedTask;
        }

        private static AsyncPolicy CreateRetryPolicy()
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    RetryCount,
                    retryAttempt => TimeSpan.FromMilliseconds(RetryDelayMilliseconds * retryAttempt));
        }

        private static string NormalizeError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return "Erro nao informado.";

            var trimmed = message.Trim();
            return trimmed.Length <= MaxErrorLength
                ? trimmed
                : trimmed[..MaxErrorLength];
        }
        #endregion
    }
}
