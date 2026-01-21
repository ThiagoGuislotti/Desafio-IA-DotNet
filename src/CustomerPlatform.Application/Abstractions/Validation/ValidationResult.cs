namespace CustomerPlatform.Application.Abstractions.Validation
{
    /// <summary>
    /// Resultado de validacao.
    /// </summary>
    public sealed class ValidationResult
    {
        #region Public Properties
        /// <summary>
        /// Indica se a validacao e valida.
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>
        /// Erros encontrados.
        /// </summary>
        public IReadOnlyCollection<ValidationError> Errors { get; }
        #endregion

        #region Constructors
        private ValidationResult(IReadOnlyCollection<ValidationError> errors)
        {
            Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Cria um resultado valido.
        /// </summary>
        /// <returns>Resultado valido.</returns>
        public static ValidationResult Success()
        {
            return new ValidationResult(Array.Empty<ValidationError>());
        }

        /// <summary>
        /// Cria um resultado invalido.
        /// </summary>
        /// <param name="errors">Erros de validacao.</param>
        /// <returns>Resultado invalido.</returns>
        public static ValidationResult Failure(IEnumerable<ValidationError> errors)
        {
            if (errors is null)
                throw new ArgumentNullException(nameof(errors));

            return new ValidationResult(errors.ToArray());
        }
        #endregion
    }
}