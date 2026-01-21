using CustomerPlatform.Application.Abstractions.Messaging;
using CustomerPlatform.Domain.Events;
using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.Data.Context.Entities;
using System.Text.Json;

namespace CustomerPlatform.Infrastructure.Messaging
{
    /// <summary>
    /// Escrita de eventos na outbox.
    /// </summary>
    public sealed class OutboxWriter : IOutboxWriter
    {
        #region Static Variables
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        #endregion

        #region Variables
        private readonly CustomerPlatformDbContext _dbContext;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="dbContext">DbContext da infraestrutura.</param>
        public OutboxWriter(CustomerPlatformDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public Task EnqueueAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            if (domainEvent is null)
                throw new ArgumentNullException(nameof(domainEvent));

            var message = EventMessageFactory.Create(domainEvent);
            var payload = JsonSerializer.Serialize(message, SerializerOptions);
            var outbox = new OutboxEvent(message.EventId, message.EventType, payload, message.OccurredAt);

            _dbContext.OutboxEvents.Add(outbox);
            return Task.CompletedTask;
        }
        #endregion
    }
}
