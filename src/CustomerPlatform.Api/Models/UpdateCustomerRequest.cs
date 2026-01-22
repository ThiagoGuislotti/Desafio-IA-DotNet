using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Enums;
using System.Text.Json.Serialization;

namespace CustomerPlatform.Api.Models
{
    /// <summary>
    /// Requisicao para atualizar cliente.
    /// </summary>
    public sealed class UpdateCustomerRequest
    {
        #region Public Properties
        /// <summary>
        /// Tipo do cliente.
        /// </summary>
        [JsonPropertyName("tipo")]
        public TipoCliente CustomerType { get; init; }

        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        [JsonPropertyName("nome")]
        public string FullName { get; init; } = string.Empty;

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
        /// Data de nascimento (apenas PF).
        /// </summary>
        [JsonPropertyName("dataNascimento")]
        public DateOnly? BirthDate { get; init; }

        /// <summary>
        /// Endereco do cliente.
        /// </summary>
        [JsonPropertyName("endereco")]
        public AddressDto Address { get; init; } = new AddressDto();
        #endregion
    }
}