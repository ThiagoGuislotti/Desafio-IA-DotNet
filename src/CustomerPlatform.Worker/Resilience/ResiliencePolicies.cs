using Polly;

namespace CustomerPlatform.Worker.Resilience
{
    /// <summary>
    /// Fabrica de politicas de resiliencia do Worker.
    /// </summary>
    public static class ResiliencePolicies
    {
        #region Constants
        private const int RetryCount = 3;
        private const int RetryDelayMilliseconds = 200;
        private const int TimeoutSeconds = 3;
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Cria politica de retry para publicacao no RabbitMQ.
        /// </summary>
        /// <returns>Policy configurada para retry.</returns>
        public static AsyncPolicy CreateRabbitPublishPolicy()
        {
            return CreateRetryPolicy();
        }

        /// <summary>
        /// Cria politica para operacoes no ElasticSearch.
        /// </summary>
        /// <returns>Policy configurada para retry e timeout.</returns>
        public static AsyncPolicy CreateElasticPolicy()
        {
            return Policy.WrapAsync(CreateRetryPolicy(), CreateTimeoutPolicy());
        }
        #endregion

        #region Private Methods/Operators
        private static AsyncPolicy CreateRetryPolicy()
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    RetryCount,
                    retryAttempt => TimeSpan.FromMilliseconds(RetryDelayMilliseconds * retryAttempt));
        }

        private static AsyncPolicy CreateTimeoutPolicy()
        {
            return Policy.TimeoutAsync(TimeSpan.FromSeconds(TimeoutSeconds));
        }
        #endregion
    }
}