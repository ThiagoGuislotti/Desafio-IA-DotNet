using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.DTOs;
using MediatR;

namespace CustomerPlatform.Application.Cqrs.Commands
{
    /// <summary>
    /// Comando para atualizar cliente pessoa fisica.
    /// </summary>
    public sealed class UpdateIndividualCustomerCommand : IRequest<Result<CustomerDto>>
    {
        #region Public Properties
        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// Email do cliente.
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        public string Phone { get; init; } = string.Empty;

        /// <summary>
        /// Data de nascimento do cliente.
        /// </summary>
        public DateOnly BirthDate { get; init; }

        /// <summary>
        /// Endereco do cliente.
        /// </summary>
        public AddressDto Address { get; init; } = new AddressDto();
        #endregion
    }
}