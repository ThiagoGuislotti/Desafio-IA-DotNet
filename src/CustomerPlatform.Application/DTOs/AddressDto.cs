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
        public string Street { get; init; } = string.Empty;

        /// <summary>
        /// Numero do endereco.
        /// </summary>
        public string Number { get; init; } = string.Empty;

        /// <summary>
        /// Complemento do endereco.
        /// </summary>
        public string? Complement { get; init; }

        /// <summary>
        /// CEP do endereco.
        /// </summary>
        public string PostalCode { get; init; } = string.Empty;

        /// <summary>
        /// Cidade do endereco.
        /// </summary>
        public string City { get; init; } = string.Empty;

        /// <summary>
        /// Estado do endereco.
        /// </summary>
        public string State { get; init; } = string.Empty;
        #endregion
    }
}
