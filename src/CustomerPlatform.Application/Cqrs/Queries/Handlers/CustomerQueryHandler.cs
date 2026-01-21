using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Application.Abstractions.Validation;
using CustomerPlatform.Application.Cqrs.Queries;
using CustomerPlatform.Application.DTOs;
using MediatR;

namespace CustomerPlatform.Application.Cqrs.Queries.Handlers
{
    /// <summary>
    /// Handler para consultas de cliente.
    /// </summary>
    public sealed class CustomerQueryHandler
        : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>,
          IRequestHandler<SearchCustomersQuery, Result<IReadOnlyCollection<CustomerDto>>>
    {
        #region Variables
        private readonly ICustomerSearchService _searchService;
        private readonly IValidator<GetCustomerByIdQuery> _getByIdValidator;
        private readonly IValidator<SearchCustomersQuery> _searchValidator;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="searchService">Servico de busca.</param>
        /// <param name="getByIdValidator">Validador da consulta por id.</param>
        /// <param name="searchValidator">Validador da busca.</param>
        public CustomerQueryHandler(
            ICustomerSearchService searchService,
            IValidator<GetCustomerByIdQuery> getByIdValidator,
            IValidator<SearchCustomersQuery> searchValidator)
        {
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
            _getByIdValidator = getByIdValidator ?? throw new ArgumentNullException(nameof(getByIdValidator));
            _searchValidator = searchValidator ?? throw new ArgumentNullException(nameof(searchValidator));
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Executa a consulta de cliente por identificador.
        /// </summary>
        /// <param name="request">Consulta.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        public async Task<Result<CustomerDto>> Handle(
            GetCustomerByIdQuery request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<CustomerDto>.Failure("Requisicao obrigatoria.");

            var validation = _getByIdValidator.Validate(request);
            if (!validation.IsValid)
                return Result<CustomerDto>.Failure(validation);

            return await _searchService
                .GetByIdAsync(request.Id, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Executa a busca simples de clientes.
        /// </summary>
        /// <param name="request">Consulta.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        public async Task<Result<IReadOnlyCollection<CustomerDto>>> Handle(
            SearchCustomersQuery request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<IReadOnlyCollection<CustomerDto>>.Failure("Requisicao obrigatoria.");

            var validation = _searchValidator.Validate(request);
            if (!validation.IsValid)
                return Result<IReadOnlyCollection<CustomerDto>>.Failure(validation);

            var criteria = new CustomerSearchCriteria
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Name = request.Name,
                Document = request.Document,
                Email = request.Email,
                Phone = request.Phone,
                CustomerType = request.CustomerType,
            };

            return await _searchService
                .SearchAsync(criteria, cancellationToken)
                .ConfigureAwait(false);
        }
        #endregion
    }
}
