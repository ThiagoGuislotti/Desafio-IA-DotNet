using System.Text.Json.Serialization;

namespace CustomerPlatform.Application.DTOs
{
    /// <summary>
    /// Dados de endereco para entrada e saida.
    /// </summary>
    public sealed class AddressDto
    {
        #region Public Properties
        /// <summary>
        /// Logradouro do endereco.
        /// </summary>
        [JsonPropertyName("logradouro")]
        public string Street { get; init; } = string.Empty;

        /// <summary>
        /// Numero do endereco.
        /// </summary>
        [JsonPropertyName("numero")]
        public string Number { get; init; } = string.Empty;

        /// <summary>
        /// Complemento do endereco.
        /// </summary>
        [JsonPropertyName("complemento")]
        public string? Complement { get; init; }

        /// <summary>
        /// CEP do endereco.
        /// </summary>
        [JsonPropertyName("cep")]
        public string PostalCode { get; init; } = string.Empty;

        /// <summary>
        /// Cidade do endereco.
        /// </summary>
        [JsonPropertyName("cidade")]
        public string City { get; init; } = string.Empty;

        /// <summary>
        /// Estado do endereco.
        /// </summary>
        [JsonPropertyName("estado")]
        public string State { get; init; } = string.Empty;
        #endregion
    }
}