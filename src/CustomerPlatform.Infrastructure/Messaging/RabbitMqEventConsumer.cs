using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace CustomerPlatform.Infrastructure.Messaging
{
    /// <summary>
    /// Consumidor RabbitMQ para eventos.
    /// </summary>
    public sealed class RabbitMqEventConsumer : IDisposable
    {
        #region Static Variables
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
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
        public RabbitMqEventConsumer(IOptions<RabbitMqOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _connection = RabbitMqConnectionFactory.Create(_options);
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(_options.ExchangeName, _options.ExchangeType, durable: true, autoDelete: false);
            _channel.BasicQos(0, _options.PrefetchCount, false);
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Inicia o consumo de eventos.
        /// </summary>
        /// <param name="queueName">Fila de consumo.</param>
        /// <param name="handler">Handler do evento.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tarefa assincrona.</returns>
        public Task StartAsync(
            string queueName,
            Func<EventMessage, CancellationToken, Task> handler,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Fila obrigatoria.", nameof(queueName));

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: queueName, exchange: _options.ExchangeName, routingKey: "#");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (_, args) =>
            {
                try
                {
                    var message = JsonSerializer.Deserialize<EventMessage>(
                        args.Body.ToArray(),
                        SerializerOptions);

                    if (message is null)
                        throw new InvalidOperationException("Mensagem invalida.");

                    await handler(message, cancellationToken).ConfigureAwait(false);
                    _channel.BasicAck(args.DeliveryTag, false);
                }
                catch
                {
                    _channel.BasicNack(args.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
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
    }
}
