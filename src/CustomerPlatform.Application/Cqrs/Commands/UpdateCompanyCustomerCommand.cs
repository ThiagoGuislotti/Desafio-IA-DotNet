using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.DTOs;
using MediatR;

namespace CustomerPlatform.Application.Cqrs.Commands
{
    /// <summary>
    /// Comando para atualizar cliente pessoa juridica.
    /// </summary>
    public sealed class UpdateCompanyCustomerCommand : IRequest<Result<CustomerDto>>
    {
        #region Public Properties
        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Razao social do cliente.
        /// </summary>
        public string CorporateName { get; init; } = string.Empty;

        /// <summary>
        /// Nome fantasia do cliente.
        /// </summary>
        public string TradeName { get; init; } = string.Empty;

        /// <summary>
        /// Email corporativo.
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        public string Phone { get; init; } = string.Empty;

        /// <summary>
        /// Endereco do cliente.
        /// </summary>
        public AddressDto Address { get; init; } = new AddressDto();
        #endregion
    }
}
