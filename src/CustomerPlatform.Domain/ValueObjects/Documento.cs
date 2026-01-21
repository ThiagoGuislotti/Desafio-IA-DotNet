using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Exceptions;

namespace CustomerPlatform.Domain.ValueObjects
{
    /// <summary>
    /// Documento CPF ou CNPJ.
    /// </summary>
    public sealed class Documento : ValueObject
    {
        #region Public Properties
        /// <summary>
        /// Numero do documento.
        /// </summary>
        public string Numero { get; }

        /// <summary>
        /// Tipo do cliente associado ao documento.
        /// </summary>
        public TipoCliente TipoCliente { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="numero">Numero do documento.</param>
        /// <exception cref="InvalidValueException">Quando o documento e invalido.</exception>
        public Documento(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new InvalidValueException("Documento invalido.");

            var normalized = NormalizeDocumento(numero);

            if (!normalized.All(char.IsDigit))
                throw new InvalidValueException("Documento deve conter apenas digitos.");

            if (normalized.Length == 11)
            {
                if (!IsValidCpf(normalized))
                    throw new InvalidValueException("CPF invalido.");

                TipoCliente = TipoCliente.PF;
                Numero = normalized;
                return;
            }

            if (normalized.Length == 14)
            {
                if (!IsValidCnpj(normalized))
                    throw new InvalidValueException("CNPJ invalido.");

                TipoCliente = TipoCliente.PJ;
                Numero = normalized;
                return;
            }

            throw new InvalidValueException("Documento deve ter 11 ou 14 digitos.");
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Retorna o documento como texto.
        /// </summary>
        /// <returns>Numero do documento.</returns>
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
        private static string NormalizeDocumento(string numero)
        {
            return numero
                .Trim()
                .Replace(".", "")
                .Replace("-", "")
                .Replace("/", "");
        }

        private static bool IsValidCpf(string cpf)
        {
            if (cpf.Length != 11)
                return false;

            for (var j = 0; j < 10; j++)
                if (new string(char.Parse(j.ToString()), 11) == cpf)
                    return false;

            var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var tempCpf = cpf[..9];
            var soma = 0;

            for (var index = 0; index < 9; index++)
                soma += int.Parse(tempCpf[index].ToString()) * multiplicador1[index];

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (var index = 0; index < 10; index++)
                soma += int.Parse(tempCpf[index].ToString()) * multiplicador2[index];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto;
            return cpf.EndsWith(digito, StringComparison.Ordinal);
        }

        private static bool IsValidCnpj(string cnpj)
        {
            if (cnpj.Length != 14)
                return false;

            var multiplicador1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var tempCnpj = cnpj[..12];
            var soma = 0;

            for (var i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;

            for (var i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            digito += resto;
            return cnpj.EndsWith(digito, StringComparison.Ordinal);
        }
        #endregion
    }
}