using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.Domain.ValueObjects;

namespace CustomerPlatform.Domain.Entities
{
    /// <summary>
    /// Entidade base para clientes.
    /// </summary>
    public abstract class Customer
    {
        #region Public Properties
        /// <summary>
        /// Identificador unico do cliente.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Email principal do cliente.
        /// </summary>
        public Email Email { get; private set; } = null!;

        /// <summary>
        /// Telefone principal do cliente.
        /// </summary>
        public Telefone Telefone { get; private set; } = null!;

        /// <summary>
        /// Endereco principal do cliente.
        /// </summary>
        public Endereco Endereco { get; private set; } = null!;

        /// <summary>
        /// Data de criacao do registro.
        /// </summary>
        public DateTime DataCriacao { get; private set; }

        /// <summary>
        /// Data da ultima atualizacao do registro.
        /// </summary>
        public DateTime? DataAtualizacao { get; private set; }

        /// <summary>
        /// Tipo do cliente.
        /// </summary>
        public abstract TipoCliente TipoCliente { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor protegido para EF/materializacao.
        /// </summary>
        protected Customer()
        {
        }

        /// <summary>
        /// Inicializa uma nova instancia de <see cref="Customer"/> com identificador interno.
        /// </summary>
        /// <param name="email">Email principal.</param>
        /// <param name="telefone">Telefone principal.</param>
        /// <param name="endereco">Endereco principal.</param>
        /// <exception cref="RequiredFieldException">Quando email, telefone ou endereco sao invalidos.</exception>
        protected Customer(Email email, Telefone telefone, Endereco endereco)
        {
            if (email is null)
                throw new RequiredFieldException(nameof(email));

            if (telefone is null)
                throw new RequiredFieldException(nameof(telefone));

            if (endereco is null)
                throw new RequiredFieldException(nameof(endereco));

            Id = Guid.NewGuid();
            Email = email;
            Telefone = telefone;
            Endereco = endereco;
            DataCriacao = DateTime.UtcNow;
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Retorna o documento principal do cliente.
        /// </summary>
        /// <returns>Documento principal.</returns>
        public abstract string GetDocumento();

        /// <summary>
        /// Retorna o nome principal do cliente.
        /// </summary>
        /// <returns>Nome principal.</returns>
        public abstract string GetNome();
        #endregion

        #region Protected Methods/Operators
        /// <summary>
        /// Atualiza dados de contato do cliente.
        /// </summary>
        /// <param name="email">Email principal.</param>
        /// <param name="telefone">Telefone principal.</param>
        /// <param name="endereco">Endereco principal.</param>
        protected void AtualizarContato(Email email, Telefone telefone, Endereco endereco)
        {
            if (email is null)
                throw new RequiredFieldException(nameof(email));

            if (telefone is null)
                throw new RequiredFieldException(nameof(telefone));

            if (endereco is null)
                throw new RequiredFieldException(nameof(endereco));

            Email = email;
            Telefone = telefone;
            Endereco = endereco;
            DataAtualizacao = DateTime.UtcNow;
        }
        #endregion
    }
}