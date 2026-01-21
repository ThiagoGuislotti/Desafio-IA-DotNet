using CustomerPlatform.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace CustomerPlatform.Domain.ValueObjects
{
    /// <summary>
    /// Email do cliente.
    /// </summary>
    public sealed class Email : ValueObject
    {
        #region Constants
        private const string EmailPattern = @"^([a-z0-9]+(?:[._-][a-z0-9]+)*)@([a-z0-9]+(?:[.-][a-z0-9]+)*\.[a-z]{2,})$";
        #endregion

        #region Static Variables
        private static readonly Regex EmailRegex = new Regex(EmailPattern, RegexOptions.Compiled);
        #endregion

        #region Public Properties
        /// <summary>
        /// Endereco de email.
        /// </summary>
        public string Endereco { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="endereco">Endereco de email.</param>
        /// <exception cref="InvalidValueException">Quando o email e invalido.</exception>
        public Email(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                throw new InvalidValueException("Email invalido.");

            var normalized = endereco.Trim();

            if (!EmailRegex.IsMatch(normalized))
                throw new InvalidValueException($"Email invalido: {normalized}.");

            Endereco = normalized;
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Retorna o email como texto.
        /// </summary>
        /// <returns>Endereco de email.</returns>
        public override string ToString()
        {
            return Endereco;
        }
        #endregion

        #region Protected Methods/Operators
        /// <inheritdoc />
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Endereco;
        }
        #endregion
    }
}