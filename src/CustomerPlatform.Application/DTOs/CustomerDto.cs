using CustomerPlatform.Domain.Enums;
using System.Text.Json.Serialization;

namespace CustomerPlatform.Application.DTOs
{
    /// <summary>
    /// Dados de cliente para retorno da aplicacao.
    /// </summary>
    public sealed class CustomerDto
    {
        #region Public Properties
        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        /// <summary>
        /// Tipo do cliente.
        /// </summary>
        [JsonPropertyName("tipo")]
        public TipoCliente CustomerType { get; init; }

        /// <summary>
        /// Documento do cliente.
        /// </summary>
        [JsonPropertyName("documento")]
        public string Document { get; init; } = string.Empty;

        /// <summary>
        /// Nome principal do cliente.
        /// </summary>
        [JsonPropertyName("nome")]
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Nome fantasia quando aplicavel.
        /// </summary>
        [JsonPropertyName("nomeFantasia")]
        public string? TradeName { get; init; }

        /// <summary>
        /// Data de nascimento quando aplicavel.
        /// </summary>
        [JsonPropertyName("dataNascimento")]
        public DateOnly? BirthDate { get; init; }

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
        /// Endereco do cliente.
        /// </summary>
        [JsonPropertyName("endereco")]
        public AddressDto Address { get; init; } = new AddressDto();

        /// <summary>
        /// Data de criacao do registro.
        /// </summary>
        [JsonPropertyName("criadoEm")]
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// Data da ultima atualizacao.
        /// </summary>
        [JsonPropertyName("atualizadoEm")]
        public DateTime? UpdatedAt { get; init; }
        #endregion
    }
}