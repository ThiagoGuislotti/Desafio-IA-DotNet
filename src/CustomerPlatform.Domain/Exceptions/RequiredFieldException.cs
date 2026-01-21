namespace CustomerPlatform.Domain.Exceptions
{
    /// <summary>
    /// Excecao para campos obrigatorios invalidos.
    /// </summary>
    public sealed class RequiredFieldException : DomainException
    {
        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="fieldName">Nome do campo.</param>
        public RequiredFieldException(string fieldName)
            : base($"Campo obrigatorio invalido: {fieldName}.")
        {
        }
        #endregion
    }
}