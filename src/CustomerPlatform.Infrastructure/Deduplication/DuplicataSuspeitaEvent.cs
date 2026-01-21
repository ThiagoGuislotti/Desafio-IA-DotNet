using CustomerPlatform.Domain.Events;
using CustomerPlatform.Domain.Exceptions;

namespace CustomerPlatform.Infrastructure.Deduplication
{
    /// <summary>
    /// Evento publicado quando uma duplicata e detectada.
    /// </summary>
    public sealed class DuplicataSuspeitaEvent : IDomainEvent
    {
        #region Constants
        /// <summary>
        /// Nome do evento.
        /// </summary>
        public const string EventTypeName = "DuplicataSuspeita";
        #endregion

        #region Public Properties
        /// <inheritdoc />
        public Guid EventId { get; }

        /// <inheritdoc />
        public DateTime OccurredAt { get; }

        /// <inheritdoc />
        public string EventType => EventTypeName;

        /// <summary>
        /// Identificador do cliente analisado.
        /// </summary>
        public Guid ClienteId { get; }

        /// <summary>
        /// Identificador do cliente candidato.
        /// </summary>
        public Guid ClienteDuplicadoId { get; }

        /// <summary>
        /// Score de similaridade.
        /// </summary>
        public decimal Score { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="clienteId">Identificador do cliente.</param>
        /// <param name="clienteDuplicadoId">Identificador do candidato.</param>
        /// <param name="score">Score calculado.</param>
        /// <param name="eventId">Identificador do evento.</param>
        /// <param name="occurredAt">Data e hora de ocorrencia.</param>
        public DuplicataSuspeitaEvent(
            Guid clienteId,
            Guid clienteDuplicadoId,
            decimal score,
            Guid? eventId = null,
            DateTime? occurredAt = null)
        {
            if (clienteId == Guid.Empty)
                throw new RequiredFieldException(nameof(clienteId));

            if (clienteDuplicadoId == Guid.Empty)
                throw new RequiredFieldException(nameof(clienteDuplicadoId));

            if (score <= 0)
                throw new RequiredFieldException(nameof(score));

            ClienteId = clienteId;
            ClienteDuplicadoId = clienteDuplicadoId;
            Score = score;
            EventId = eventId ?? Guid.NewGuid();
            OccurredAt = occurredAt ?? DateTime.UtcNow;
        }
        #endregion
    }
}
