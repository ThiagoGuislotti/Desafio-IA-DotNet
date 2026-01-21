using CustomerPlatform.Domain.Enums;

namespace CustomerPlatform.Application.Abstractions.Search
{
    /// <summary>
    /// Criterios simples de busca de clientes.
    /// </summary>
    public sealed class CustomerSearchCriteria
    {
        #region Public Properties
        /// <summary>
        /// Nome para busca.
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Documento para busca.
        /// </summary>
        public string? Document { get; init; }

        /// <summary>
        /// Email para busca.
        /// </summary>
        public string? Email { get; init; }

        /// <summary>
        /// Telefone para busca.
        /// </summary>
        public string? Phone { get; init; }

        /// <summary>
        /// Tipo de cliente.
        /// </summary>
        public TipoCliente? CustomerType { get; init; }

        /// <summary>
        /// Numero da pagina.
        /// </summary>
        public int PageNumber { get; init; } = 1;

        /// <summary>
        /// Tamanho da pagina.
        /// </summary>
        public int PageSize { get; init; } = 20;
        #endregion
    }
}
