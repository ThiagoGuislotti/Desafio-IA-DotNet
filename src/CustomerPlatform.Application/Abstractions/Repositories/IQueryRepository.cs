using CustomerPlatform.Application.Abstractions.Paginations;
using System.Linq.Expressions;

namespace CustomerPlatform.Application.Abstractions.Repositories
{
    /// <summary>
    /// Repositorio generico de leitura.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade.</typeparam>
    public interface IQueryRepository<TEntity>
        where TEntity : class
    {
        #region Public Methods/Operators
        /// <summary>
        /// Retorna a query base para composicao.
        /// </summary>
        /// <returns>Query base.</returns>
        IQueryable<TEntity> GetQueryable();

        /// <summary>
        /// Busca entidade pela chave primaria.
        /// </summary>
        /// <param name="keyValues">Valores da chave.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entidade encontrada ou <c>null</c>.</returns>
        Task<TEntity?> FindAsync(object[] keyValues, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica existencia de entidades.
        /// </summary>
        /// <param name="predicate">Filtro opcional.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns><c>true</c> se existe.</returns>
        Task<bool> ExistsAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retorna o primeiro registro conforme filtro.
        /// </summary>
        /// <param name="orderBy">Ordenacao opcional.</param>
        /// <param name="predicate">Filtro opcional.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entidade encontrada ou <c>null</c>.</returns>
        Task<TEntity?> SearchFirstOrDefaultAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retorna o primeiro registro projetado conforme filtro.
        /// </summary>
        /// <typeparam name="TSelector">Tipo projetado.</typeparam>
        /// <param name="selector">Selector da projecao.</param>
        /// <param name="orderBy">Ordenacao opcional.</param>
        /// <param name="predicate">Filtro opcional.</param>
        /// <param name="enableQueryDistinct">Habilita distinct.</param>
        /// <param name="orderBySelector">Ordenacao do selector.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Registro projetado ou <c>null</c>.</returns>
        Task<TSelector?> SearchFirstOrDefaultAsync<TSelector>(
            Expression<Func<TEntity, TSelector>> selector,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            bool enableQueryDistinct = false,
            Func<IQueryable<TSelector>, IOrderedQueryable<TSelector>>? orderBySelector = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retorna entidades em stream conforme filtro.
        /// </summary>
        /// <param name="orderBy">Ordenacao opcional.</param>
        /// <param name="predicate">Filtro opcional.</param>
        /// <returns>Stream de entidades.</returns>
        IAsyncEnumerable<TEntity> SearchEnumerableAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null);

        /// <summary>
        /// Retorna entidades projetadas em stream conforme filtro.
        /// </summary>
        /// <typeparam name="TSelector">Tipo projetado.</typeparam>
        /// <param name="selector">Selector da projecao.</param>
        /// <param name="orderBy">Ordenacao opcional.</param>
        /// <param name="predicate">Filtro opcional.</param>
        /// <param name="enableQueryDistinct">Habilita distinct.</param>
        /// <param name="orderBySelector">Ordenacao do selector.</param>
        /// <returns>Stream de resultados.</returns>
        IAsyncEnumerable<TSelector> SearchEnumerableAsync<TSelector>(
            Expression<Func<TEntity, TSelector>> selector,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            bool enableQueryDistinct = false,
            Func<IQueryable<TSelector>, IOrderedQueryable<TSelector>>? orderBySelector = null);

        /// <summary>
        /// Retorna pagina de entidades conforme filtro.
        /// </summary>
        /// <param name="pagination">Parametros de pagina.</param>
        /// <param name="predicate">Filtro opcional.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Pagina de entidades.</returns>
        Task<PagedList<TEntity>> SearchPagedListAsync(
            Pagination? pagination = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retorna pagina projetada conforme filtro.
        /// </summary>
        /// <typeparam name="TSelector">Tipo projetado.</typeparam>
        /// <param name="selector">Selector da projecao.</param>
        /// <param name="pagination">Parametros de pagina.</param>
        /// <param name="predicate">Filtro opcional.</param>
        /// <param name="enableQueryDistinct">Habilita distinct.</param>
        /// <param name="orderBySelector">Ordenacao do selector.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Pagina de resultados.</returns>
        Task<PagedList<TSelector>> SearchPagedListAsync<TSelector>(
            Expression<Func<TEntity, TSelector>> selector,
            Pagination? pagination = null,
            Expression<Func<TEntity, bool>>? predicate = null,
            bool enableQueryDistinct = false,
            Func<IQueryable<TSelector>, IOrderedQueryable<TSelector>>? orderBySelector = null,
            CancellationToken cancellationToken = default);
        #endregion
    }
}