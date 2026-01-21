namespace CustomerPlatform.Infrastructure.Data.Context.Entities
{
    /// <summary>
    /// Registro de suspeita de duplicidade.
    /// </summary>
    public sealed class DuplicateSuspicion
    {
        #region Public Properties
        /// <summary>
        /// Identificador interno.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Identificador do cliente avaliado.
        /// </summary>
        public Guid CustomerId { get; private set; }

        /// <summary>
        /// Identificador do cliente candidato.
        /// </summary>
        public Guid CandidateCustomerId { get; private set; }

        /// <summary>
        /// Score de similaridade.
        /// </summary>
        public decimal Score { get; private set; }

        /// <summary>
        /// Descricao do motivo.
        /// </summary>
        public string? Reason { get; private set; }

        /// <summary>
        /// Data de criacao.
        /// </summary>
        public DateTime CreatedAt { get; private set; }
        #endregion

        #region Constructors
        private DuplicateSuspicion()
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="customerId">Identificador do cliente.</param>
        /// <param name="candidateCustomerId">Identificador do candidato.</param>
        /// <param name="score">Score de similaridade.</param>
        /// <param name="reason">Motivo descritivo.</param>
        public DuplicateSuspicion(
            Guid customerId,
            Guid candidateCustomerId,
            decimal score,
            string? reason)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            CandidateCustomerId = candidateCustomerId;
            Score = score;
            Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
            CreatedAt = DateTime.UtcNow;
        }
        #endregion
    }
}
