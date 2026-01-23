using CustomerPlatform.Application.Abstractions.Messaging;
using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.Data.Context.Entities;
using Microsoft.Extensions.Options;

namespace CustomerPlatform.Infrastructure.Deduplication
{
    /// <summary>
    /// Processador de deduplicacao baseado em busca e score.
    /// </summary>
    public sealed class DeduplicationProcessor
    {
        #region Variables
        private readonly CustomerPlatformDbContext _dbContext;
        private readonly ICustomerSearchService _searchService;
        private readonly IEventPublisher _eventPublisher;
        private readonly DeduplicationOptions _options;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="dbContext">DbContext da infraestrutura.</param>
        /// <param name="searchService">Servico de busca.</param>
        /// <param name="eventPublisher">Publicador de eventos.</param>
        /// <param name="options">Opcoes de deduplicacao.</param>
        public DeduplicationProcessor(
            CustomerPlatformDbContext dbContext,
            ICustomerSearchService searchService,
            IEventPublisher eventPublisher,
            IOptions<DeduplicationOptions> options)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Processa a deduplicacao do cliente informado.
        /// </summary>
        /// <param name="customer">Cliente a analisar.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado do processamento.</returns>
        public async Task<Result> ProcessAsync(CustomerDto customer, CancellationToken cancellationToken = default)
        {
            if (customer is null)
                return Result.Failure("Cliente obrigatorio.");

            var criteria = new CustomerSearchCriteria
            {
                Name = customer.Name,
                PageNumber = 1,
                PageSize = _options.MaxCandidates
            };

            var searchResult = await _searchService.SearchAsync(criteria, cancellationToken).ConfigureAwait(false);
            if (!searchResult.IsSuccess || searchResult.Data is null)
                return Result.Failure(searchResult.Errors, searchResult.Message);

            var candidates = searchResult.Data
                .Where(candidate => candidate.Id != customer.Id)
                .ToList();

            if (candidates.Count == 0)
                return Result.Success();

            var suspicions = new List<DuplicateSuspicion>();
            foreach (var candidate in candidates)
            {
                var score = CalculateScore(customer, candidate);
                if (score < _options.Threshold)
                    continue;

                var reason = BuildReason(customer, candidate, score);
                suspicions.Add(new DuplicateSuspicion(customer.Id, candidate.Id, score, reason));
            }

            if (suspicions.Count == 0)
                return Result.Success();

            _dbContext.DuplicateSuspicions.AddRange(suspicions);
            await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            foreach (var suspicion in suspicions)
            {
                var domainEvent = new DuplicataSuspeitaEvent(
                    suspicion.CustomerId,
                    suspicion.CandidateCustomerId,
                    suspicion.Score);
                await _eventPublisher.PublishAsync(domainEvent, cancellationToken).ConfigureAwait(false);
            }

            return Result.Success();
        }
        #endregion

        #region Private Methods/Operators
        private static decimal CalculateScore(CustomerDto source, CustomerDto candidate)
        {
            var nameScore = CalculateNameSimilarity(source.Name, candidate.Name);
            var emailScore = string.Equals(source.Email, candidate.Email, StringComparison.OrdinalIgnoreCase) ? 1m : 0m;
            var phoneScore = string.Equals(source.Phone, candidate.Phone, StringComparison.OrdinalIgnoreCase) ? 1m : 0m;

            return Math.Round(
                (nameScore * 0.5m) +
                (emailScore * 0.3m) +
                (phoneScore * 0.2m),
                2);
        }

        private static decimal CalculateNameSimilarity(string source, string candidate)
        {
            var normalizedSource = NormalizeText(source);
            var normalizedCandidate = NormalizeText(candidate);

            if (normalizedSource.Length == 0 || normalizedCandidate.Length == 0)
                return 0m;

            var distance = CalculateLevenshteinDistance(normalizedSource, normalizedCandidate);
            var maxLength = Math.Max(normalizedSource.Length, normalizedCandidate.Length);
            var similarity = 1m - (decimal)distance / maxLength;
            return Math.Max(0m, similarity);
        }

        private static int CalculateLevenshteinDistance(string source, string target)
        {
            // Algoritmo classico para distancia de edicao.
            var sourceLength = source.Length;
            var targetLength = target.Length;
            var matrix = new int[sourceLength + 1, targetLength + 1];

            for (var i = 0; i <= sourceLength; i++)
                matrix[i, 0] = i;

            for (var j = 0; j <= targetLength; j++)
                matrix[0, j] = j;

            for (var i = 1; i <= sourceLength; i++)
            {
                for (var j = 1; j <= targetLength; j++)
                {
                    var cost = source[i - 1] == target[j - 1] ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }

            return matrix[sourceLength, targetLength];
        }

        private static string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : new string(value
                    .Trim()
                    .Where(char.IsLetterOrDigit)
                    .ToArray())
                    .ToUpperInvariant();
        }

        private static string BuildReason(CustomerDto source, CustomerDto candidate, decimal score)
        {
            var reasons = new List<string>
            {
                $"Score={score:0.00}"
            };

            if (string.Equals(source.Email, candidate.Email, StringComparison.OrdinalIgnoreCase))
                reasons.Add("Email igual");

            if (string.Equals(source.Phone, candidate.Phone, StringComparison.OrdinalIgnoreCase))
                reasons.Add("Telefone igual");

            return string.Join("; ", reasons);
        }
        #endregion
    }
}