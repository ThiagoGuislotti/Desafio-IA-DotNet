using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace CustomerPlatform.Application.Cqrs.Commands
{
    /// <summary>
    /// Comando para criar cliente pessoa fisica.
    /// </summary>
    public sealed class CreateIndividualCustomerCommand : IRequest<Result<CustomerDto>>
    {
        #region Public Properties
        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        [JsonPropertyName("nome")]
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// CPF do cliente.
        /// </summary>
        [JsonPropertyName("cpf")]
        public string Cpf { get; init; } = string.Empty;

        /// <summary>
        /// Email do cliente.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        [JsonPropertyName("telefone")]
        public string Phone { get; init; } = string.Empty;

        /// <summary>
        /// Data de nascimento do cliente.
        /// </summary>
        [JsonPropertyName("dataNascimento")]
        public DateOnly BirthDate { get; init; }

        /// <summary>
        /// Endereco do cliente.
        /// </summary>
        [JsonPropertyName("endereco")]
        public AddressDto Address { get; init; } = new AddressDto();
        #endregion
    }
}