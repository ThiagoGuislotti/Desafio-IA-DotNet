using System;
using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.Domain.ValueObjects;

namespace CustomerPlatform.Domain.Entities
{
    /// <summary>
    /// Cliente pessoa fisica.
    /// </summary>
    public sealed class ClientePessoaFisica : Customer
    {
        #region Public Properties
        /// <summary>
        /// Nome completo do cliente.
        /// </summary>
        public string Nome { get; private set; } = string.Empty;

        /// <summary>
        /// Documento do cliente.
        /// </summary>
        public Documento Documento { get; private set; } = null!;

        /// <summary>
        /// Data de nascimento.
        /// </summary>
        public DateOnly DataNascimento { get; private set; }

        /// <summary>
        /// CPF do cliente.
        /// </summary>
        public string CPF => Documento.Numero;

        /// <inheritdoc />
        public override TipoCliente TipoCliente => TipoCliente.PF;
        #endregion

        #region Constructors
        private ClientePessoaFisica()
        {
        }

        private ClientePessoaFisica(
            string nome,
            Documento documento,
            Email email,
            Telefone telefone,
            DateOnly dataNascimento,
            Endereco endereco)
            : base(email, telefone, endereco)
        {
            Nome = ValidarNome(nome);
            Documento = ValidarDocumento(documento);
            DataNascimento = ValidarDataNascimento(dataNascimento);
        }

        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Cria um cliente pessoa fisica valido.
        /// </summary>
        /// <param name="nome">Nome completo.</param>
        /// <param name="cpf">CPF do cliente.</param>
        /// <param name="email">Email do cliente.</param>
        /// <param name="telefone">Telefone do cliente.</param>
        /// <param name="dataNascimento">Data de nascimento.</param>
        /// <param name="endereco">Endereco do cliente.</param>
        /// <returns>Instancia valida de <see cref="ClientePessoaFisica"/>.</returns>
        public static ClientePessoaFisica Criar(
            string nome,
            string cpf,
            string email,
            string telefone,
            DateOnly dataNascimento,
            Endereco endereco)
        {
            var documento = new Documento(cpf);
            var emailValue = new Email(email);
            var telefoneValue = new Telefone(telefone);

            return new ClientePessoaFisica(
                nome,
                documento,
                emailValue,
                telefoneValue,
                dataNascimento,
                endereco);
        }

        /// <inheritdoc />
        public override string GetDocumento()
        {
            return Documento.Numero;
        }

        /// <inheritdoc />
        public override string GetNome()
        {
            return Nome;
        }
        #endregion

        #region Private Methods/Operators
        private static string ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new RequiredFieldException(nameof(nome));

            return nome.Trim();
        }

        private static Documento ValidarDocumento(Documento documento)
        {
            if (documento is null)
                throw new InvalidValueException("Documento invalido.");

            if (documento.TipoCliente != TipoCliente.PF)
                throw new InvalidValueException("CPF deve ter 11 digitos.");

            return documento;
        }

        private static DateOnly ValidarDataNascimento(DateOnly dataNascimento)
        {
            if (dataNascimento == default)
                throw new RequiredFieldException(nameof(dataNascimento));

            return dataNascimento;
        }
        #endregion
    }
}
