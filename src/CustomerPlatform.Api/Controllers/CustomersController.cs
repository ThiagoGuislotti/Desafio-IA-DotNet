using CustomerPlatform.Api.Extensions;
using CustomerPlatform.Api.Models;
using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.Cqrs.Queries;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPlatform.Api.Controllers
{
    /// <summary>
    /// Controller para operacoes de clientes.
    /// </summary>
    [ApiController]
    [Route("customers")]
    public sealed class CustomersController : ControllerBase
    {
        #region Constants
        private const string PaginationHeaderName = "X-Pagination";
        #endregion

        #region Variables
        private readonly IMediator _mediator;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="mediator">Mediator.</param>
        public CustomersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Cria cliente pessoa fisica.
        /// </summary>
        /// <param name="command">Comando de criacao.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resposta HTTP.</returns>
        [HttpPost("pf")]
        public async Task<IActionResult> CreateIndividual(
            [FromBody] CreateIndividualCustomerCommand command,
            CancellationToken cancellationToken)
        {
            if (command is null)
                return this.ToActionResult(Result<CustomerDto>.Failure("Requisicao obrigatoria."));

            var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
            return this.ToActionResult(result, StatusCodes.Status201Created);
        }

        /// <summary>
        /// Cria cliente pessoa juridica.
        /// </summary>
        /// <param name="command">Comando de criacao.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resposta HTTP.</returns>
        [HttpPost("pj")]
        public async Task<IActionResult> CreateCompany(
            [FromBody] CreateCompanyCustomerCommand command,
            CancellationToken cancellationToken)
        {
            if (command is null)
                return this.ToActionResult(Result<CustomerDto>.Failure("Requisicao obrigatoria."));

            var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
            return this.ToActionResult(result, StatusCodes.Status201Created);
        }

        /// <summary>
        /// Atualiza cliente.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <param name="request">Dados da atualizacao.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resposta HTTP.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateCustomerRequest request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return this.ToActionResult(Result<CustomerDto>.Failure("Requisicao obrigatoria."));

            if (id == Guid.Empty)
                return this.ToActionResult(Result<CustomerDto>.Failure("Id obrigatorio."));

            return request.CustomerType switch
            {
                TipoCliente.PF => await UpdateIndividualAsync(id, request, cancellationToken).ConfigureAwait(false),
                TipoCliente.PJ => await UpdateCompanyAsync(id, request, cancellationToken).ConfigureAwait(false),
                _ => this.ToActionResult(Result<CustomerDto>.Failure("Tipo de cliente invalido."))
            };
        }

        /// <summary>
        /// Busca clientes via ElasticSearch.
        /// </summary>
        /// <param name="query">Criterios de busca.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resposta HTTP.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] SearchCustomersQuery query,
            CancellationToken cancellationToken)
        {
            if (query is null)
                return this.ToActionResult(Result<IReadOnlyCollection<CustomerDto>>.Failure("Requisicao obrigatoria."));

            var result = await _mediator.Send(query, cancellationToken).ConfigureAwait(false);
            if (result.IsSuccess)
            {
                var headerValue = $"{{pageSize={query.PageSize},currentPage={query.PageNumber}}}";
                Response.Headers[PaginationHeaderName] = headerValue;
            }
            return this.ToActionResult(result);
        }
        #endregion

        #region Private Methods/Operators
        private async Task<IActionResult> UpdateIndividualAsync(
            Guid id,
            UpdateCustomerRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateIndividualCustomerCommand
            {
                Id = id,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                BirthDate = request.BirthDate ?? default,
                Address = request.Address
            };

            var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
            return this.ToActionResult(result);
        }

        private async Task<IActionResult> UpdateCompanyAsync(
            Guid id,
            UpdateCustomerRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateCompanyCustomerCommand
            {
                Id = id,
                CorporateName = request.CorporateName,
                TradeName = request.TradeName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address
            };

            var result = await _mediator.Send(command, cancellationToken).ConfigureAwait(false);
            return this.ToActionResult(result);
        }
        #endregion
    }
}