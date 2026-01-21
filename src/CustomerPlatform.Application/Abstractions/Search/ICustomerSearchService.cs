using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.DTOs;

namespace CustomerPlatform.Application.Abstractions.Search
{
    /// <summary>
    /// Contrato de busca para indexacao e consulta de clientes.
    /// </summary>
    public interface ICustomerSearchService
    {
        #region Public Methods/Operators
        /// <summary>
        /// Indexa um cliente no mecanismo de busca.
        /// </summary>
        /// <param name="customer">Cliente a indexar.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        Task<Result> IndexAsync(CustomerDto customer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera cliente por identificador.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado com o cliente encontrado.</returns>
        Task<Result<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executa busca de clientes.
        /// </summary>
        /// <param name="criteria">Criterios de busca.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado com clientes encontrados.</returns>
        Task<Result<IReadOnlyCollection<CustomerDto>>> SearchAsync(
            CustomerSearchCriteria criteria,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
