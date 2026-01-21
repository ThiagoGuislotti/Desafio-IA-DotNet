namespace CustomerPlatform.Infrastructure.Messaging
{
    /// <summary>
    /// Opcoes de configuracao do RabbitMQ.
    /// </summary>
    public sealed class RabbitMqOptions
    {
        #region Constants
        /// <summary>
        /// Nome padrao da exchange.
        /// </summary>
        public const string DefaultExchangeName = "customerplatform.events";
        #endregion

        #region Public Properties
        /// <summary>
        /// Connection string AMQP.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Nome da exchange.
        /// </summary>
        public string ExchangeName { get; set; } = DefaultExchangeName;

        /// <summary>
        /// Tipo da exchange.
        /// </summary>
        public string ExchangeType { get; set; } = "topic";

        /// <summary>
        /// Nome da fila padrao para consumo.
        /// </summary>
        public string QueueName { get; set; } = "customerplatform.events";

        /// <summary>
        /// Prefetch para consumidores.
        /// </summary>
        public ushort PrefetchCount { get; set; } = 10;
        #endregion
    }
}
