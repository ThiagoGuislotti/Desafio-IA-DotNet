using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Infrastructure.Search;
using Polly;

namespace CustomerPlatform.Worker.Resilience
{
    /// <summary>
    /// Wrapper resiliente para operacoes no ElasticSearch.
    /// </summary>
    public sealed class ResilientCustomerSearchService : ICustomerSearchService
    {
        #region Variables
        private readonly ElasticCustomerSearchService _inner;
        private readonly AsyncPolicy _policy;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="inner">Servico interno de busca.</param>
        public ResilientCustomerSearchService(ElasticCustomerSearchService inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _policy = ResiliencePolicies.CreateElasticPolicy();
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public Task<Result> IndexAsync(CustomerDto customer, CancellationToken cancellationToken = default)
        {
            return _policy.ExecuteAsync(
                ct => _inner.IndexAsync(customer, ct),
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<Result<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _policy.ExecuteAsync(
                ct => _inner.GetByIdAsync(id, ct),
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<Result<IReadOnlyCollection<CustomerDto>>> SearchAsync(
            CustomerSearchCriteria criteria,
            CancellationToken cancellationToken = default)
        {
            return _policy.ExecuteAsync(
                ct => _inner.SearchAsync(criteria, ct),
                cancellationToken);
        }
        #endregion

        #region Private Methods/Operators
        #endregion
    }
}