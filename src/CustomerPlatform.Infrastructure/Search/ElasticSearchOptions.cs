namespace CustomerPlatform.Infrastructure.Search
{
    /// <summary>
    /// Opcoes de configuracao do ElasticSearch.
    /// </summary>
    public sealed class ElasticSearchOptions
    {
        #region Public Properties
        /// <summary>
        /// Url do ElasticSearch.
        /// </summary>
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Nome do indice principal.
        /// </summary>
        public string IndexName { get; set; } = "customers";

        /// <summary>
        /// Usuario para autenticacao basica.
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Senha para autenticacao basica.
        /// </summary>
        public string? Password { get; set; }
        #endregion
    }
}