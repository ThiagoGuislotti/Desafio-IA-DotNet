using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.DTOs;

namespace CustomerPlatform.Application.Abstractions.Deduplication
{
    /// <summary>
    /// Contrato para processamento de deduplicacao.
    /// </summary>
    public interface IDeduplicationService
    {
        #region Public Methods/Operators
        /// <summary>
        /// Enfileira o cliente para deduplicacao.
        /// </summary>
        /// <param name="customer">Cliente a ser analisado.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        Task<Result> EnqueueAsync(CustomerDto customer, CancellationToken cancellationToken = default);
        #endregion
    }
}