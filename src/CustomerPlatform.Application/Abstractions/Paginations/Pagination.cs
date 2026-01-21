namespace CustomerPlatform.Application.Abstractions.Paginations
{
    /// <summary>
    /// Parametros de paginacao.
    /// </summary>
    public sealed class Pagination
    {
        #region Public Properties
        /// <summary>
        /// Numero da pagina.
        /// </summary>
        public int PageNumber { get; init; } = 1;

        /// <summary>
        /// Tamanho da pagina.
        /// </summary>
        public int PageSize { get; init; } = 20;
        #endregion
    }
}