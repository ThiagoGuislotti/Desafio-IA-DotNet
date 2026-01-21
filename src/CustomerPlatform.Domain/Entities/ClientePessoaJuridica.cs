using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.Domain.ValueObjects;

namespace CustomerPlatform.Domain.Entities
{
    /// <summary>
    /// Cliente pessoa juridica.
    /// </summary>
    public sealed class ClientePessoaJuridica : Customer
    {
        #region Public Properties
        /// <summary>
        /// Razao social do cliente.
        /// </summary>
        public string RazaoSocial { get; private set; } = string.Empty;

        /// <summary>
        /// Nome fantasia do cliente.
        /// </summary>
        public string NomeFantasia { get; private set; } = string.Empty;

        /// <summary>
        /// Documento do cliente.
        /// </summary>
        public Documento Documento { get; private set; } = null!;

        /// <summary>
        /// CNPJ do cliente.
        /// </summary>
        public string CNPJ => Documento.Numero;

        /// <inheritdoc />
        public override TipoCliente TipoCliente => TipoCliente.PJ;
        #endregion

        #region Constructors
        private ClientePessoaJuridica()
        {
        }

        private ClientePessoaJuridica(
            string razaoSocial,
            string nomeFantasia,
            Documento documento,
            Email email,
            Telefone telefone,
            Endereco endereco)
            : base(email, telefone, endereco)
        {
            RazaoSocial = ValidarRazaoSocial(razaoSocial);
            NomeFantasia = ValidarNomeFantasia(nomeFantasia);
            Documento = ValidarDocumento(documento);
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Cria um cliente pessoa juridica valido.
        /// </summary>
        /// <param name="razaoSocial">Razao social.</param>
        /// <param name="nomeFantasia">Nome fantasia.</param>
        /// <param name="cnpj">CNPJ do cliente.</param>
        /// <param name="email">Email do cliente.</param>
        /// <param name="telefone">Telefone do cliente.</param>
        /// <param name="endereco">Endereco do cliente.</param>
        /// <returns>Instancia valida de <see cref="ClientePessoaJuridica"/>.</returns>
        public static ClientePessoaJuridica Criar(
            string razaoSocial,
            string nomeFantasia,
            string cnpj,
            string email,
            string telefone,
            Endereco endereco)
        {
            var documento = new Documento(cnpj);
            var emailValue = new Email(email);
            var telefoneValue = new Telefone(telefone);

            return new ClientePessoaJuridica(
                razaoSocial,
                nomeFantasia,
                documento,
                emailValue,
                telefoneValue,
                endereco);
        }

        /// <summary>
        /// Atualiza dados do cliente pessoa juridica.
        /// </summary>
        /// <param name="razaoSocial">Razao social.</param>
        /// <param name="nomeFantasia">Nome fantasia.</param>
        /// <param name="email">Email corporativo.</param>
        /// <param name="telefone">Telefone do cliente.</param>
        /// <param name="endereco">Endereco do cliente.</param>
        public void Atualizar(
            string razaoSocial,
            string nomeFantasia,
            string email,
            string telefone,
            Endereco endereco)
        {
            var emailValue = new Email(email);
            var telefoneValue = new Telefone(telefone);

            RazaoSocial = ValidarRazaoSocial(razaoSocial);
            NomeFantasia = ValidarNomeFantasia(nomeFantasia);
            AtualizarContato(emailValue, telefoneValue, endereco);
        }

        /// <inheritdoc />
        public override string GetDocumento()
        {
            return Documento.Numero;
        }

        /// <inheritdoc />
        public override string GetNome()
        {
            return RazaoSocial;
        }
        #endregion

        #region Private Methods/Operators
        private static string ValidarRazaoSocial(string razaoSocial)
        {
            if (string.IsNullOrWhiteSpace(razaoSocial))
                throw new RequiredFieldException(nameof(razaoSocial));

            return razaoSocial.Trim();
        }

        private static string ValidarNomeFantasia(string nomeFantasia)
        {
            if (string.IsNullOrWhiteSpace(nomeFantasia))
                throw new RequiredFieldException(nameof(nomeFantasia));

            return nomeFantasia.Trim();
        }

        private static Documento ValidarDocumento(Documento documento)
        {
            if (documento is null)
                throw new InvalidValueException("Documento invalido.");

            if (documento.TipoCliente != TipoCliente.PJ)
                throw new InvalidValueException("CNPJ deve ter 14 digitos.");

            return documento;
        }
        #endregion
    }
}