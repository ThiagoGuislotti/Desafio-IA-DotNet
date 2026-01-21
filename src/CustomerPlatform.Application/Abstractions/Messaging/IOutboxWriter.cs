using CustomerPlatform.Domain.Events;

namespace CustomerPlatform.Application.Abstractions.Messaging
{
    /// <summary>
    /// Contrato para escrita na Outbox.
    /// </summary>
    public interface IOutboxWriter
    {
        #region Public Methods/Operators
        /// <summary>
        /// Registra um evento para publicacao posterior.
        /// </summary>
        /// <param name="domainEvent">Evento de dominio.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Tarefa assincrona.</returns>
        Task EnqueueAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
        #endregion
    }
}