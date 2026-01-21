using CustomerPlatform.Domain.Enums;

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
        public Guid Id { get; init; }

        /// <summary>
        /// Tipo do cliente.
        /// </summary>
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
        public AddressDto Address { get; init; } = new AddressDto();

        /// <summary>
        /// Data de criacao do registro.
        /// </summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// Data da ultima atualizacao.
        /// </summary>
        public DateTime? UpdatedAt { get; init; }
        #endregion
    }
}
