using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.DTOs;
using MediatR;

namespace CustomerPlatform.Application.Cqrs.Queries
{
    /// <summary>
    /// Consulta para obter cliente por identificador.
    /// </summary>
    public sealed class GetCustomerByIdQuery : IRequest<Result<CustomerDto>>
    {
        #region Public Properties
        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public Guid Id { get; init; }
        #endregion
    }
}