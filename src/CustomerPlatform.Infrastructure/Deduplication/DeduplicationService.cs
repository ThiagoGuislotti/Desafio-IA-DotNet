using CustomerPlatform.Application.Abstractions.Deduplication;
using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.DTOs;

namespace CustomerPlatform.Infrastructure.Deduplication
{
    /// <summary>
    /// Implementacao da deduplicacao na infraestrutura.
    /// </summary>
    public sealed class DeduplicationService : IDeduplicationService
    {
        #region Variables
        private readonly DeduplicationProcessor _processor;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="processor">Processador de deduplicacao.</param>
        public DeduplicationService(DeduplicationProcessor processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public async Task<Result> EnqueueAsync(CustomerDto customer, CancellationToken cancellationToken = default)
        {
            return await _processor.ProcessAsync(customer, cancellationToken).ConfigureAwait(false);
        }
        #endregion
    }
}