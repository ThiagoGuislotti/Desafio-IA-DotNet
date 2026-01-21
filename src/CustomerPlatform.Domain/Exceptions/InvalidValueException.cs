namespace CustomerPlatform.Domain.Exceptions
{
    /// <summary>
    /// Excecao para valor invalido.
    /// </summary>
    public sealed class InvalidValueException : DomainException
    {
        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="message">Mensagem de erro.</param>
        public InvalidValueException(string message)
            : base(message)
        {
        }
        #endregion
    }
}
