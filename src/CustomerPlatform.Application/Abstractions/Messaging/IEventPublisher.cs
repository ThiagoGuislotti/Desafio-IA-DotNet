using CustomerPlatform.Domain.Events;

namespace CustomerPlatform.Application.Abstractions.Messaging
{
    /// <summary>
    /// Contrato de publicacao de eventos.
    /// </summary>
    public interface IEventPublisher
    {
        #region Public Methods/Operators
        /// <summary>
        /// Publica um evento de dominio.
        /// </summary>
        /// <param name="domainEvent">Evento de dominio.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tarefa assincrona.</returns>
        Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
        #endregion
    }
}