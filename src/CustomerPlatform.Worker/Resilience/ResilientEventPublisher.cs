using CustomerPlatform.Application.Abstractions.Messaging;
using CustomerPlatform.Domain.Events;
using CustomerPlatform.Infrastructure.Messaging;
using Polly;

namespace CustomerPlatform.Worker.Resilience
{
    /// <summary>
    /// Wrapper resiliente para publicacao no RabbitMQ.
    /// </summary>
    public sealed class ResilientEventPublisher : IEventPublisher
    {
        #region Variables
        private readonly RabbitMqEventPublisher _inner;
        private readonly AsyncPolicy _policy;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="inner">Publicador interno.</param>
        public ResilientEventPublisher(RabbitMqEventPublisher inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _policy = ResiliencePolicies.CreateRabbitPublishPolicy();
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public Task PublishAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            if (domainEvent is null)
                throw new ArgumentNullException(nameof(domainEvent));

            return _policy.ExecuteAsync(
                ct => _inner.PublishAsync(domainEvent, ct),
                cancellationToken);
        }
        #endregion

        #region Private Methods/Operators
        #endregion
    }
}