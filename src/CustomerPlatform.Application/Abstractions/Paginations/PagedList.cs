namespace CustomerPlatform.Application.Abstractions.Paginations
{
    /// <summary>
    /// Resultado paginado.
    /// </summary>
    /// <typeparam name="T">Tipo dos itens.</typeparam>
    public sealed class PagedList<T>
    {
        #region Public Properties
        /// <summary>
        /// Itens da pagina.
        /// </summary>
        public IReadOnlyCollection<T> Items { get; }

        /// <summary>
        /// Total de itens.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Numero da pagina.
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// Tamanho da pagina.
        /// </summary>
        public int PageSize { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="items">Itens retornados.</param>
        /// <param name="totalCount">Total de itens.</param>
        /// <param name="pageNumber">Numero da pagina.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        public PagedList(IReadOnlyCollection<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        #endregion
    }
}
