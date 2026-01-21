using RabbitMQ.Client;

namespace CustomerPlatform.Infrastructure.Messaging
{
    /// <summary>
    /// Fabrica de conexoes com RabbitMQ.
    /// </summary>
    internal static class RabbitMqConnectionFactory
    {
        #region Public Methods/Operators
        /// <summary>
        /// Cria uma conexao configurada.
        /// </summary>
        /// <param name="options">Opcoes do RabbitMQ.</param>
        /// <returns>Conexao ativa.</returns>
        public static IConnection Create(RabbitMqOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new InvalidOperationException("ConnectionString do RabbitMQ nao configurada.");

            var factory = new ConnectionFactory
            {
                Uri = new Uri(options.ConnectionString),
                DispatchConsumersAsync = true
            };

            return factory.CreateConnection();
        }
        #endregion
    }
}
