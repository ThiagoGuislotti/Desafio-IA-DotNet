using System;
using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Exceptions;

namespace CustomerPlatform.Domain.Events
{
    /// <summary>
    /// Evento publicado quando um cliente e criado.
    /// </summary>
    public sealed class ClienteCriado : IDomainEvent
    {
        #region Constants
        /// <summary>
        /// Nome do evento.
        /// </summary>
        public const string EventTypeName = "ClienteCriado";
        #endregion

        #region Public Properties
        /// <inheritdoc />
        public Guid EventId { get; }

        /// <inheritdoc />
        public DateTime OccurredAt { get; }

        /// <inheritdoc />
        public string EventType => EventTypeName;

        /// <summary>
        /// Identificador do cliente.
        /// </summary>
        public Guid ClienteId { get; }

        /// <summary>
        /// Tipo do cliente.
        /// </summary>
        public TipoCliente TipoCliente { get; }

        /// <summary>
        /// Documento do cliente.
        /// </summary>
        public string Documento { get; }

        /// <summary>
        /// Nome do cliente.
        /// </summary>
        public string Nome { get; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="clienteId">Identificador do cliente.</param>
        /// <param name="tipoCliente">Tipo do cliente.</param>
        /// <param name="documento">Documento do cliente.</param>
        /// <param name="nome">Nome do cliente.</param>
        /// <param name="eventId">Identificador do evento.</param>
        /// <param name="occurredAt">Data e hora de ocorrencia.</param>
        public ClienteCriado(
            Guid clienteId,
            TipoCliente tipoCliente,
            string documento,
            string nome,
            Guid? eventId = null,
            DateTime? occurredAt = null)
        {
            if (clienteId == Guid.Empty)
                throw new RequiredFieldException(nameof(clienteId));

            if (string.IsNullOrWhiteSpace(documento))
                throw new RequiredFieldException(nameof(documento));

            if (string.IsNullOrWhiteSpace(nome))
                throw new RequiredFieldException(nameof(nome));

            ClienteId = clienteId;
            TipoCliente = tipoCliente;
            Documento = documento.Trim();
            Nome = nome.Trim();
            EventId = eventId ?? Guid.NewGuid();
            OccurredAt = occurredAt ?? DateTime.UtcNow;
        }
        #endregion
    }
}
