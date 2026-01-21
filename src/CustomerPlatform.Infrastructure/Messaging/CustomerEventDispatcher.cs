using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Events;
using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.Deduplication;
using Microsoft.EntityFrameworkCore;

namespace CustomerPlatform.Infrastructure.Messaging
{
    /// <summary>
    /// Despachante de eventos para indexacao e deduplicacao.
    /// </summary>
    public sealed class CustomerEventDispatcher
    {
        #region Variables
        private readonly CustomerPlatformDbContext _dbContext;
        private readonly ICustomerSearchService _searchService;
        private readonly DeduplicationProcessor _deduplicationProcessor;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="dbContext">DbContext da infraestrutura.</param>
        /// <param name="searchService">Servico de busca.</param>
        /// <param name="deduplicationProcessor">Processador de deduplicacao.</param>
        public CustomerEventDispatcher(
            CustomerPlatformDbContext dbContext,
            ICustomerSearchService searchService,
            DeduplicationProcessor deduplicationProcessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
            _deduplicationProcessor = deduplicationProcessor ?? throw new ArgumentNullException(nameof(deduplicationProcessor));
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Processa eventos de cliente para indexacao e deduplicacao.
        /// </summary>
        /// <param name="message">Mensagem recebida.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado do processamento.</returns>
        public async Task<Result> HandleAsync(EventMessage message, CancellationToken cancellationToken = default)
        {
            if (message is null)
                return Result.Failure("Mensagem obrigatoria.");

            if (message.EventType != ClienteCriado.EventTypeName &&
                message.EventType != ClienteAtualizado.EventTypeName)
                return Result.Success();

            var customer = await _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(current => current.Id == message.CustomerId, cancellationToken)
                .ConfigureAwait(false);

            if (customer is null)
                return Result.Failure("Cliente nao encontrado para indexacao.");

            var dto = CustomerDtoMapper.Map(customer);

            var indexResult = await _searchService.IndexAsync(dto, cancellationToken).ConfigureAwait(false);
            if (!indexResult.IsSuccess)
                return Result.Failure(indexResult.Errors, indexResult.Message);

            return await _deduplicationProcessor.ProcessAsync(dto, cancellationToken).ConfigureAwait(false);
        }
        #endregion
    }
}