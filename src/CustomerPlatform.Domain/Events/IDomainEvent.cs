namespace CustomerPlatform.Domain.Events
{
    /// <summary>
    /// Contrato base para eventos de dominio.
    /// </summary>
    public interface IDomainEvent
    {
        #region Public Properties
        /// <summary>
        /// Identificador do evento.
        /// </summary>
        Guid EventId { get; }

        /// <summary>
        /// Data e hora de ocorrencia.
        /// </summary>
        DateTime OccurredAt { get; }

        /// <summary>
        /// Tipo do evento.
        /// </summary>
        string EventType { get; }
        #endregion
    }
}