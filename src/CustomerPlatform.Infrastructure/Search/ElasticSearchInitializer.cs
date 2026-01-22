using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;

namespace CustomerPlatform.Infrastructure.Search
{
    /// <summary>
    /// Inicializa indices do ElasticSearch.
    /// </summary>
    public sealed class ElasticSearchInitializer
    {
        #region Variables
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="options">Opcoes do ElasticSearch.</param>
        public ElasticSearchInitializer(IOptions<ElasticSearchOptions> options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            _indexName = options.Value.IndexName;
            _client = ElasticClientFactory.Create(options.Value);
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Garante que o indice exista.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da inicializacao.</returns>
        public async Task<bool> EnsureIndexAsync(CancellationToken cancellationToken = default)
        {
            var exists = await _client.Indices.ExistsAsync(_indexName, cancellationToken).ConfigureAwait(false);
            if (exists.ApiCallDetails?.HttpStatusCode == 200)
                return true;

            var response = await _client.Indices.CreateAsync(_indexName, cancellationToken)
                .ConfigureAwait(false);
            return response.IsValidResponse;
        }
        #endregion
    }
}