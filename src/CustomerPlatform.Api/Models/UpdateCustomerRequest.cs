using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Enums;

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
        public TipoCliente CustomerType { get; init; }

        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        public string FullName { get; init; } = string.Empty;

        /// <summary>
        /// Razao social do cliente.
        /// </summary>
        public string CorporateName { get; init; } = string.Empty;

        /// <summary>
        /// Nome fantasia do cliente.
        /// </summary>
        public string TradeName { get; init; } = string.Empty;

        /// <summary>
        /// Email do cliente.
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        public string Phone { get; init; } = string.Empty;

        /// <summary>
        /// Data de nascimento (apenas PF).
        /// </summary>
        public DateOnly? BirthDate { get; init; }

        /// <summary>
        /// Endereco do cliente.
        /// </summary>
        public AddressDto Address { get; init; } = new AddressDto();
        #endregion
    }
}