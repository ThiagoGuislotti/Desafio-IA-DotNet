using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.DTOs;
using MediatR;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("razaoSocial")]
        public string CorporateName { get; init; } = string.Empty;

        /// <summary>
        /// Nome fantasia do cliente.
        /// </summary>
        [JsonPropertyName("nomeFantasia")]
        public string TradeName { get; init; } = string.Empty;

        /// <summary>
        /// CNPJ do cliente.
        /// </summary>
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; init; } = string.Empty;

        /// <summary>
        /// Email corporativo.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        [JsonPropertyName("telefone")]
        public string Phone { get; init; } = string.Empty;

        /// <summary>
        /// Endereco do cliente.
        /// </summary>
        [JsonPropertyName("endereco")]
        public AddressDto Address { get; init; } = new AddressDto();
        #endregion
    }
}