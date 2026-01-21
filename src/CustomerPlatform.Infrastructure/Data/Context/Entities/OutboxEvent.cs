namespace CustomerPlatform.Infrastructure.Data.Context.Entities
{
    /// <summary>
    /// Evento persistido para publicacao posterior.
    /// </summary>
    public sealed class OutboxEvent
    {
        #region Public Properties
        /// <summary>
        /// Identificador interno.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Identificador do evento de dominio.
        /// </summary>
        public Guid EventId { get; private set; }

        /// <summary>
        /// Tipo do evento.
        /// </summary>
        public string EventType { get; private set; } = string.Empty;

        /// <summary>
        /// Payload serializado.
        /// </summary>
        public string Payload { get; private set; } = string.Empty;

        /// <summary>
        /// Data e hora de ocorrencia.
        /// </summary>
        public DateTime OccurredAt { get; private set; }

        /// <summary>
        /// Data de criacao do registro.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Data de processamento.
        /// </summary>
        public DateTime? ProcessedAt { get; private set; }

        /// <summary>
        /// Numero de tentativas de publicacao.
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Ultimo erro registrado.
        /// </summary>
        public string? LastError { get; private set; }
        #endregion

        #region Constructors
        private OutboxEvent()
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="eventId">Identificador do evento.</param>
        /// <param name="eventType">Tipo do evento.</param>
        /// <param name="payload">Payload serializado.</param>
        /// <param name="occurredAt">Data de ocorrencia.</param>
        public OutboxEvent(Guid eventId, string eventType, string payload, DateTime occurredAt)
        {
            Id = Guid.NewGuid();
            EventId = eventId;
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            OccurredAt = occurredAt;
            CreatedAt = DateTime.UtcNow;
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Marca o evento como processado.
        /// </summary>
        public void MarkProcessed()
        {
            ProcessedAt = DateTime.UtcNow;
            LastError = null;
        }

        /// <summary>
        /// Registra falha de processamento.
        /// </summary>
        /// <param name="error">Mensagem de erro.</param>
        public void RegisterFailure(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                throw new ArgumentException("Erro obrigatorio.", nameof(error));

            RetryCount++;
            LastError = error.Trim();
        }
        #endregion
    }
}