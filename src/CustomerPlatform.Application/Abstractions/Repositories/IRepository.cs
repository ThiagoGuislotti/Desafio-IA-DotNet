using CustomerPlatform.Application.Abstractions.Results;

namespace CustomerPlatform.Application.Abstractions.Repositories
{
    /// <summary>
    /// Repositorio generico de leitura e escrita.
    /// </summary>
    /// <typeparam name="TEntity">Tipo da entidade.</typeparam>
    public interface IRepository<TEntity> : IQueryRepository<TEntity>
        where TEntity : class
    {
        #region Public Methods/Operators
        /// <summary>
        /// Insere uma entidade.
        /// </summary>
        /// <param name="entity">Entidade.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        Task<Result> InsertAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Atualiza uma entidade.
        /// </summary>
        /// <param name="entity">Entidade.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        Task<Result> UpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove uma entidade pela chave.
        /// </summary>
        /// <param name="keyValues">Valores da chave.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        Task<Result> DeleteAsync(
            object[] keyValues,
            CancellationToken cancellationToken = default);
        #endregion
    }
}