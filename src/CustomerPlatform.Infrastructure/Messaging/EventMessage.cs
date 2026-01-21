using CustomerPlatform.Domain.Enums;

namespace CustomerPlatform.Infrastructure.Messaging
{
    /// <summary>
    /// Mensagem de evento para mensageria.
    /// </summary>
    public sealed class EventMessage
    {
        #region Public Properties
        /// <summary>
        /// Identificador do evento.
        /// </summary>
        public Guid EventId { get; init; }

        /// <summary>
        /// Tipo do evento.
        /// </summary>
        public string EventType { get; init; } = string.Empty;

        /// <summary>
        /// Data e hora de ocorrencia.
        /// </summary>
        public DateTime OccurredAt { get; init; }

        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public Guid CustomerId { get; init; }

        /// <summary>
        /// Identificador do cliente candidato.
        /// </summary>
        public Guid? CandidateCustomerId { get; init; }

        /// <summary>
        /// Tipo do cliente.
        /// </summary>
        public TipoCliente? CustomerType { get; init; }

        /// <summary>
        /// Documento do cliente.
        /// </summary>
        public string? Document { get; init; }

        /// <summary>
        /// Nome principal do cliente.
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Email do cliente.
        /// </summary>
        public string? Email { get; init; }

        /// <summary>
        /// Telefone do cliente.
        /// </summary>
        public string? Phone { get; init; }

        /// <summary>
        /// Score de similaridade quando aplicavel.
        /// </summary>
        public decimal? Score { get; init; }
        #endregion
    }
}
