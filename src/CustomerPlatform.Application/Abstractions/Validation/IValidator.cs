namespace CustomerPlatform.Application.Abstractions.Validation
{
    /// <summary>
    /// Contrato para validacao de entrada.
    /// </summary>
    /// <typeparam name="T">Tipo validado.</typeparam>
    public interface IValidator<in T>
    {
        #region Public Methods/Operators
        /// <summary>
        /// Executa validacoes simples.
        /// </summary>
        /// <param name="instance">Instancia a validar.</param>
        /// <returns>Resultado da validacao.</returns>
        ValidationResult Validate(T instance);
        #endregion
    }
}