using CustomerPlatform.Application.Abstractions.Repositories;
using CustomerPlatform.Application.Abstractions.Results;

namespace CustomerPlatform.Application.Abstractions
{
    /// <summary>
    /// Contrato de unidade de trabalho.
    /// </summary>
    public interface IUnitOfWork
    {
        #region Public Methods/Operators
        /// <summary>
        /// Retorna o repositorio solicitado.
        /// </summary>
        /// <typeparam name="TEntity">Tipo da entidade.</typeparam>
        /// <returns>Repositorio generico.</returns>
        IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Confirma as alteracoes pendentes.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        Task<Result> CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Reverte alteracoes pendentes.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tarefa assincrona.</returns>
        Task RollbackAsync(CancellationToken cancellationToken = default);
        #endregion
    }
}