using CustomerPlatform.Application.Abstractions.Messaging;
using CustomerPlatform.Domain.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;

namespace CustomerPlatform.Infrastructure.Messaging
{
    /// <summary>
    /// Publicador RabbitMQ para eventos de dominio.
    /// </summary>
    public sealed class RabbitMqEventPublisher : IEventPublisher, IDisposable
    {
        #region Static Variables
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        #endregion

        #region Variables
        private readonly RabbitMqOptions _options;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private bool _disposed;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="options">Opcoes do RabbitMQ.</param>
        public RabbitMqEventPublisher(IOptions<RabbitMqOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _connection = RabbitMqConnectionFactory.Create(_options);
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_options.ExchangeName, _options.ExchangeType, durable: true, autoDelete: false);
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public async Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            if (domainEvent is null)
                throw new ArgumentNullException(nameof(domainEvent));

            var message = EventMessageFactory.Create(domainEvent);
            var body = JsonSerializer.SerializeToUtf8Bytes(message, SerializerOptions);

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = message.EventId.ToString();
            properties.Type = message.EventType;
            properties.Timestamp = new AmqpTimestamp(new DateTimeOffset(message.OccurredAt).ToUnixTimeSeconds());

            _channel.BasicPublish(
                exchange: _options.ExchangeName,
                routingKey: message.EventType,
                basicProperties: properties,
                body: body);

            await Task.CompletedTask.ConfigureAwait(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
                return;

            _channel.Dispose();
            _connection.Dispose();
            _disposed = true;
        }
        #endregion

        #region Private Methods/Operators
        #endregion
    }
}