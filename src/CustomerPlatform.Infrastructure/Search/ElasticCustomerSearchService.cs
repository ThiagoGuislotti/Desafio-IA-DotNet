using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Application.DTOs;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Options;
using ApplicationResult = CustomerPlatform.Application.Abstractions.Results.Result;

namespace CustomerPlatform.Infrastructure.Search
{
    /// <summary>
    /// Implementacao de busca com ElasticSearch.
    /// </summary>
    public sealed class ElasticCustomerSearchService : ICustomerSearchService
    {
        #region Variables
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="options">Opcoes do ElasticSearch.</param>
        public ElasticCustomerSearchService(IOptions<ElasticSearchOptions> options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            _indexName = options.Value.IndexName;
            _client = ElasticClientFactory.Create(options.Value);
        }
        #endregion

        #region Public Methods/Operators
        /// <inheritdoc />
        public async Task<ApplicationResult> IndexAsync(CustomerDto customer, CancellationToken cancellationToken = default)
        {
            if (customer is null)
                return ApplicationResult.Failure("Cliente obrigatorio.");

            var document = MapToDocument(customer);
            var response = await _client.IndexAsync(
                    document,
                    _indexName,
                    descriptor => descriptor
                        .Id(document.Id)
                        .Refresh(Refresh.True),
                    cancellationToken)
                .ConfigureAwait(false);

            return response.IsValidResponse
                ? ApplicationResult.Success()
                : ApplicationResult.Failure(response.ElasticsearchServerError?.Error?.Reason ?? "Falha ao indexar cliente.");
        }

        /// <inheritdoc />
        public async Task<CustomerPlatform.Application.Abstractions.Results.Result<CustomerDto>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return CustomerPlatform.Application.Abstractions.Results.Result<CustomerDto>.Failure("Id obrigatorio.");

            var response = await _client.GetAsync<CustomerSearchDocument>(
                    _indexName,
                    id.ToString("N"),
                    cancellationToken)
                .ConfigureAwait(false);

            if (!response.Found)
                return CustomerPlatform.Application.Abstractions.Results.Result<CustomerDto>.Failure("Cliente nao encontrado.");

            return CustomerPlatform.Application.Abstractions.Results.Result<CustomerDto>.Success(MapToDto(response.Source!));
        }

        /// <inheritdoc />
        public async Task<CustomerPlatform.Application.Abstractions.Results.Result<IReadOnlyCollection<CustomerDto>>> SearchAsync(
            CustomerSearchCriteria criteria,
            CancellationToken cancellationToken = default)
        {
            if (criteria is null)
                return CustomerPlatform.Application.Abstractions.Results.Result<IReadOnlyCollection<CustomerDto>>.Failure("Criterios obrigatorios.");

            var from = Math.Max(criteria.PageNumber - 1, 0) * Math.Max(criteria.PageSize, 1);
            var size = Math.Max(criteria.PageSize, 1);
            var query = BuildQuery(criteria);

            var response = await _client.SearchAsync<CustomerSearchDocument>(
                    request => request
                        .Index(_indexName)
                        .From(from)
                        .Size(size)
                        .Query(query),
                    cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsValidResponse)
                return CustomerPlatform.Application.Abstractions.Results.Result<IReadOnlyCollection<CustomerDto>>.Failure(
                    response.ElasticsearchServerError?.Error?.Reason ?? "Falha na busca.");

            var results = response.Documents
                .Select(MapToDto)
                .ToArray();

            return CustomerPlatform.Application.Abstractions.Results.Result<IReadOnlyCollection<CustomerDto>>.Success(results);
        }
        #endregion

        #region Private Methods/Operators
        private static CustomerSearchDocument MapToDocument(CustomerDto customer)
        {
            return new CustomerSearchDocument
            {
                Id = customer.Id.ToString("N"),
                CustomerType = customer.CustomerType,
                Document = customer.Document,
                Name = customer.Name,
                TradeName = customer.TradeName,
                BirthDate = customer.BirthDate,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = new AddressDocument
                {
                    Street = customer.Address.Street,
                    Number = customer.Address.Number,
                    Complement = customer.Address.Complement,
                    PostalCode = customer.Address.PostalCode,
                    City = customer.Address.City,
                    State = customer.Address.State,
                },
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt,
            };
        }

        private static CustomerDto MapToDto(CustomerSearchDocument document)
        {
            return new CustomerDto
            {
                Id = Guid.Parse(document.Id),
                CustomerType = document.CustomerType,
                Document = document.Document,
                Name = document.Name,
                TradeName = document.TradeName,
                BirthDate = document.BirthDate,
                Email = document.Email,
                Phone = document.Phone,
                Address = new AddressDto
                {
                    Street = document.Address.Street,
                    Number = document.Address.Number,
                    Complement = document.Address.Complement,
                    PostalCode = document.Address.PostalCode,
                    City = document.Address.City,
                    State = document.Address.State,
                },
                CreatedAt = document.CreatedAt,
                UpdatedAt = document.UpdatedAt,
            };
        }

        private static Query BuildQuery(CustomerSearchCriteria criteria)
        {
            var filters = new List<Query>();
            var should = new List<Query>();

            if (!string.IsNullOrWhiteSpace(criteria.Document))
                filters.Add(new TermQuery("document") { Value = criteria.Document.Trim() });

            if (!string.IsNullOrWhiteSpace(criteria.Email))
                filters.Add(new WildcardQuery("email") { Value = $"*{criteria.Email.Trim()}*" });

            if (!string.IsNullOrWhiteSpace(criteria.Phone))
                filters.Add(new WildcardQuery("phone") { Value = $"*{criteria.Phone.Trim()}*" });

            if (criteria.CustomerType.HasValue)
                filters.Add(new TermQuery("customerType") { Value = (int)criteria.CustomerType.Value });

            if (!string.IsNullOrWhiteSpace(criteria.Name))
            {
                var name = criteria.Name.Trim();
                should.Add(new MatchQuery("name") { Query = name, Fuzziness = new Fuzziness("AUTO") });
                should.Add(new MatchQuery("tradeName") { Query = name, Fuzziness = new Fuzziness("AUTO") });
            }

            if (filters.Count == 0 && should.Count == 0)
                return new MatchAllQuery();

            return new BoolQuery
            {
                Filter = filters.Count > 0 ? filters : null,
                Should = should.Count > 0 ? should : null,
                MinimumShouldMatch = should.Count > 0 ? 1 : null
            };
        }
        #endregion
    }
}