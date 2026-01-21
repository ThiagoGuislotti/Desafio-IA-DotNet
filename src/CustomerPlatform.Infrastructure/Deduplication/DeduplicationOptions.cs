namespace CustomerPlatform.Infrastructure.Deduplication
{
    /// <summary>
    /// Opcoes de deduplicacao.
    /// </summary>
    public sealed class DeduplicationOptions
    {
        #region Public Properties
        /// <summary>
        /// Score minimo para registrar suspeita.
        /// </summary>
        public decimal Threshold { get; set; } = 0.85m;

        /// <summary>
        /// Maximo de candidatos avaliados.
        /// </summary>
        public int MaxCandidates { get; set; } = 20;
        #endregion
    }
}