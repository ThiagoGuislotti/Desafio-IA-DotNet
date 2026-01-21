namespace CustomerPlatform.Domain.Exceptions
{
    /// <summary>
    /// Excecao base do dominio.
    /// </summary>
    public class DomainException : Exception
    {
        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        public DomainException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        /// <param name="innerException">Excecao interna.</param>
        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #endregion
    }
}