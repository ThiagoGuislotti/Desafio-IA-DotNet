using CustomerPlatform.Domain.Enums;
using System.Text.Json.Serialization;

namespace CustomerPlatform.Infrastructure.Search
{
    /// <summary>
    /// Documento de cliente no indice de busca.
    /// </summary>
    public sealed class CustomerSearchDocument
    {
        #region Public Properties
        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public string Id { get; init; } = string.Empty;

        /// <summary>
        /// Tipo do cliente.
        /// </summary>
        [JsonConverter(typeof(TipoClienteJsonConverter))]
        public TipoCliente CustomerType { get; init; }

        /// <summary>
        /// Documento do cliente.
        /// </summary>
        public string Document { get; init; } = string.Empty;

        /// <summary>
        /// Nome principal do cliente.
        /// </summary>
        public string Name { get; init; } = string.Empty;

        /// <summary>
        /// Nome fantasia quando aplicavel.
        /// </summary>
        public string? TradeName { get; init; }

        /// <summary>
        /// Data de nascimento quando aplicavel.
        /// </summary>
        public DateOnly? BirthDate { get; init; }

        /// <summary>
        /// Email do cliente.
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        public string Phone { get; init; } = string.Empty;

        /// <summary>
        /// Endereco do cliente.
        /// </summary>
        public AddressDocument Address { get; init; } = new AddressDocument();

        /// <summary>
        /// Data de criacao.
        /// </summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// Data de atualizacao.
        /// </summary>
        public DateTime? UpdatedAt { get; init; }
        #endregion
    }

    /// <summary>
    /// Documento de endereco no indice de busca.
    /// </summary>
    public sealed class AddressDocument
    {
        #region Public Properties
        /// <summary>
        /// Logradouro.
        /// </summary>
        public string Street { get; init; } = string.Empty;

        /// <summary>
        /// Numero.
        /// </summary>
        public string Number { get; init; } = string.Empty;

        /// <summary>
        /// Complemento.
        /// </summary>
        public string? Complement { get; init; }

        /// <summary>
        /// CEP.
        /// </summary>
        public string PostalCode { get; init; } = string.Empty;

        /// <summary>
        /// Cidade.
        /// </summary>
        public string City { get; init; } = string.Empty;

        /// <summary>
        /// Estado.
        /// </summary>
        public string State { get; init; } = string.Empty;
        #endregion
    }
}