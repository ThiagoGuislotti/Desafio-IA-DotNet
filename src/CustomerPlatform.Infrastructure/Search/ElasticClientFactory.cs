using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace CustomerPlatform.Infrastructure.Search
{
    /// <summary>
    /// Fabrica de clientes do ElasticSearch.
    /// </summary>
    internal static class ElasticClientFactory
    {
        #region Public Methods/Operators
        /// <summary>
        /// Cria um cliente configurado.
        /// </summary>
        /// <param name="options">Opcoes de configuracao.</param>
        /// <returns>Cliente configurado.</returns>
        public static ElasticsearchClient Create(ElasticSearchOptions options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
                throw new InvalidOperationException("ConnectionString do ElasticSearch nao configurada.");

            var settings = new ElasticsearchClientSettings(new Uri(options.ConnectionString))
                .DefaultIndex(options.IndexName);

            if (!string.IsNullOrWhiteSpace(options.Username))
            {
                settings = settings.Authentication(
                    new BasicAuthentication(options.Username, options.Password ?? string.Empty));
            }

            return new ElasticsearchClient(settings);
        }
        #endregion
    }
}