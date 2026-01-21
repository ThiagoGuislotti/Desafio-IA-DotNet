using CustomerPlatform.Domain.Exceptions;

namespace CustomerPlatform.Domain.ValueObjects
{
    /// <summary>
    /// Telefone do cliente.
    /// </summary>
    public sealed class Telefone : ValueObject
    {
        #region Public Properties
        /// <summary>
        /// Numero do telefone.
        /// </summary>
        public string Numero { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="numero">Numero do telefone.</param>
        /// <exception cref="InvalidValueException">Quando o telefone e invalido.</exception>
        public Telefone(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new InvalidValueException("Telefone invalido.");

            var normalized = NormalizeTelefone(numero);

            if (!normalized.All(char.IsDigit))
                throw new InvalidValueException("Telefone deve conter apenas digitos.");

            if (normalized.Length != 10 && normalized.Length != 11)
                throw new InvalidValueException("Telefone deve ter 10 ou 11 digitos.");

            Numero = normalized;
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Retorna o telefone como texto.
        /// </summary>
        /// <returns>Numero do telefone.</returns>
        public override string ToString()
        {
            return Numero;
        }
        #endregion

        #region Protected Methods/Operators
        /// <inheritdoc />
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Numero;
        }
        #endregion

        #region Private Methods/Operators
        private static string NormalizeTelefone(string numero)
        {
            return numero
                .Trim()
                .Replace(" ", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("-", "");
        }
        #endregion
    }
}