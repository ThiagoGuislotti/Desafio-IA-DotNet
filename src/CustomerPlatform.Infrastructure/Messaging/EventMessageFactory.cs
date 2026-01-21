using CustomerPlatform.Domain.Events;
using CustomerPlatform.Infrastructure.Deduplication;

namespace CustomerPlatform.Infrastructure.Messaging
{
    /// <summary>
    /// Fabrica de mensagens para eventos.
    /// </summary>
    internal static class EventMessageFactory
    {
        #region Public Methods/Operators
        /// <summary>
        /// Cria uma mensagem a partir do evento informado.
        /// </summary>
        /// <param name="domainEvent">Evento de dominio.</param>
        /// <returns>Mensagem serializavel.</returns>
        public static EventMessage Create(IDomainEvent domainEvent)
        {
            if (domainEvent is null)
                throw new ArgumentNullException(nameof(domainEvent));

            return domainEvent switch
            {
                ClienteCriado created => new EventMessage
                {
                    EventId = created.EventId,
                    EventType = created.EventType,
                    OccurredAt = created.OccurredAt,
                    CustomerId = created.ClienteId,
                    CustomerType = created.TipoCliente,
                    Document = created.Documento,
                    Name = created.Nome,
                },
                ClienteAtualizado updated => new EventMessage
                {
                    EventId = updated.EventId,
                    EventType = updated.EventType,
                    OccurredAt = updated.OccurredAt,
                    CustomerId = updated.ClienteId,
                    CustomerType = updated.TipoCliente,
                    Document = updated.Documento,
                    Name = updated.Nome,
                },
                DuplicataSuspeitaEvent duplicated => new EventMessage
                {
                    EventId = duplicated.EventId,
                    EventType = duplicated.EventType,
                    OccurredAt = duplicated.OccurredAt,
                    CustomerId = duplicated.ClienteId,
                    CandidateCustomerId = duplicated.ClienteDuplicadoId,
                    Score = duplicated.Score,
                },
                _ => new EventMessage
                {
                    EventId = domainEvent.EventId,
                    EventType = domainEvent.EventType,
                    OccurredAt = domainEvent.OccurredAt,
                }
            };
        }
        #endregion
    }
}
