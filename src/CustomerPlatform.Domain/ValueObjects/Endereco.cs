using System.Collections.Generic;
using CustomerPlatform.Domain.Exceptions;

namespace CustomerPlatform.Domain.ValueObjects
{
    /// <summary>
    /// Endereco do cliente.
    /// </summary>
    public sealed class Endereco : ValueObject
    {
        #region Public Properties
        /// <summary>
        /// Logradouro do endereco.
        /// </summary>
        public string Logradouro { get; }

        /// <summary>
        /// Numero do endereco.
        /// </summary>
        public string Numero { get; }

        /// <summary>
        /// Complemento do endereco.
        /// </summary>
        public string? Complemento { get; }

        /// <summary>
        /// CEP do endereco.
        /// </summary>
        public string Cep { get; }

        /// <summary>
        /// Cidade do endereco.
        /// </summary>
        public string Cidade { get; }

        /// <summary>
        /// Estado do endereco.
        /// </summary>
        public string Estado { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="logradouro">Logradouro do endereco.</param>
        /// <param name="numero">Numero do endereco.</param>
        /// <param name="complemento">Complemento do endereco.</param>
        /// <param name="cep">CEP do endereco.</param>
        /// <param name="cidade">Cidade do endereco.</param>
        /// <param name="estado">Estado do endereco.</param>
        /// <exception cref="InvalidValueException">Quando o endereco e invalido.</exception>
        public Endereco(
            string logradouro,
            string numero,
            string? complemento,
            string cep,
            string cidade,
            string estado)
        {
            Logradouro = ValidarObrigatorio(logradouro, nameof(logradouro));
            Numero = ValidarObrigatorio(numero, nameof(numero));
            Cep = ValidarObrigatorio(cep, nameof(cep));
            Cidade = ValidarObrigatorio(cidade, nameof(cidade));
            Estado = ValidarObrigatorio(estado, nameof(estado));
            Complemento = string.IsNullOrWhiteSpace(complemento) ? null : complemento.Trim();
        }
        #endregion

        #region Protected Methods/Operators
        /// <inheritdoc />
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Logradouro;
            yield return Numero;
            yield return Complemento;
            yield return Cep;
            yield return Cidade;
            yield return Estado;
        }
        #endregion

        #region Private Methods/Operators
        private static string ValidarObrigatorio(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new InvalidValueException($"Campo obrigatorio invalido: {campo}.");

            return valor.Trim();
        }
        #endregion
    }
}
