using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.DTOs;
using MediatR;

namespace CustomerPlatform.Application.Cqrs.Commands
{
    /// <summary>
    /// Comando para criar cliente pessoa juridica.
    /// </summary>
    public sealed class CreateCompanyCustomerCommand : IRequest<Result<CustomerDto>>
    {
        #region Public Properties
        /// <summary>
        /// Razao social do cliente.
        /// </summary>
        public string CorporateName { get; init; } = string.Empty;

        /// <summary>
        /// Nome fantasia do cliente.
        /// </summary>
        public string TradeName { get; init; } = string.Empty;

        /// <summary>
        /// CNPJ do cliente.
        /// </summary>
        public string Cnpj { get; init; } = string.Empty;

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