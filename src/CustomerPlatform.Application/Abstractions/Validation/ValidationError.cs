namespace CustomerPlatform.Application.Abstractions.Validation
{
    /// <summary>
    /// Representa um erro de validacao.
    /// </summary>
    public sealed class ValidationError
    {
        #region Public Properties
        /// <summary>
        /// Nome da propriedade invalida.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Mensagem de erro.
        /// </summary>
        public string ErrorMessage { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="propertyName">Nome da propriedade.</param>
        /// <param name="errorMessage">Mensagem de erro.</param>
        public ValidationError(string propertyName, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentException("PropertyName obrigatorio.", nameof(propertyName));

            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("ErrorMessage obrigatorio.", nameof(errorMessage));

            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }
        #endregion
    }
}