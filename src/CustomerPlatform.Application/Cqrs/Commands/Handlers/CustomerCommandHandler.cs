using CustomerPlatform.Application.Abstractions;
using CustomerPlatform.Application.Abstractions.Messaging;
using CustomerPlatform.Application.Abstractions.Repositories;
using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Validation;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Entities;
using CustomerPlatform.Domain.Events;
using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.Domain.ValueObjects;
using MediatR;

namespace CustomerPlatform.Application.Cqrs.Commands.Handlers
{
    /// <summary>
    /// Handler para comandos de cliente.
    /// </summary>
    public sealed class CustomerCommandHandler
        : IRequestHandler<CreateIndividualCustomerCommand, Result<CustomerDto>>,
        IRequestHandler<CreateCompanyCustomerCommand, Result<CustomerDto>>,
        IRequestHandler<UpdateIndividualCustomerCommand, Result<CustomerDto>>,
        IRequestHandler<UpdateCompanyCustomerCommand, Result<CustomerDto>>
    {
        #region Variables
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerDocumentChecker _documentChecker;
        private readonly IOutboxWriter _outboxWriter;
        private readonly IValidator<CreateIndividualCustomerCommand> _individualValidator;
        private readonly IValidator<CreateCompanyCustomerCommand> _companyValidator;
        private readonly IValidator<UpdateIndividualCustomerCommand> _updateIndividualValidator;
        private readonly IValidator<UpdateCompanyCustomerCommand> _updateCompanyValidator;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="unitOfWork">Unidade de trabalho.</param>
        /// <param name="documentChecker">Verificador de duplicidade de documento.</param>
        /// <param name="outboxWriter">Escrita da outbox.</param>
        /// <param name="individualValidator">Validador para PF.</param>
        /// <param name="companyValidator">Validador para PJ.</param>
        /// <param name="updateIndividualValidator">Validador de atualizacao para PF.</param>
        /// <param name="updateCompanyValidator">Validador de atualizacao para PJ.</param>
        public CustomerCommandHandler(
            IUnitOfWork unitOfWork,
            ICustomerDocumentChecker documentChecker,
            IOutboxWriter outboxWriter,
            IValidator<CreateIndividualCustomerCommand> individualValidator,
            IValidator<CreateCompanyCustomerCommand> companyValidator,
            IValidator<UpdateIndividualCustomerCommand> updateIndividualValidator,
            IValidator<UpdateCompanyCustomerCommand> updateCompanyValidator)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _documentChecker = documentChecker ?? throw new ArgumentNullException(nameof(documentChecker));
            _outboxWriter = outboxWriter ?? throw new ArgumentNullException(nameof(outboxWriter));
            _individualValidator = individualValidator ?? throw new ArgumentNullException(nameof(individualValidator));
            _companyValidator = companyValidator ?? throw new ArgumentNullException(nameof(companyValidator));
            _updateIndividualValidator = updateIndividualValidator ?? throw new ArgumentNullException(nameof(updateIndividualValidator));
            _updateCompanyValidator = updateCompanyValidator ?? throw new ArgumentNullException(nameof(updateCompanyValidator));
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Executa o comando de criacao de cliente pessoa fisica.
        /// </summary>
        /// <param name="request">Comando de criacao.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        public async Task<Result<CustomerDto>> Handle(
            CreateIndividualCustomerCommand request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<CustomerDto>.Failure("Requisicao obrigatoria.");

            var validation = _individualValidator.Validate(request);
            if (!validation.IsValid)
                return Result<CustomerDto>.Failure(validation);

            try
            {
                var address = new Endereco(
                    request.Address.Street,
                    request.Address.Number,
                    request.Address.Complement,
                    request.Address.PostalCode,
                    request.Address.City,
                    request.Address.State);

                var customer = ClientePessoaFisica.Criar(
                    request.FullName,
                    request.Cpf,
                    request.Email,
                    request.Phone,
                    request.BirthDate,
                    address);

                return await InsertCustomerAsync(customer, cancellationToken).ConfigureAwait(false);
            }
            catch (DomainException ex)
            {
                return Result<CustomerDto>.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Executa o comando de criacao de cliente pessoa juridica.
        /// </summary>
        /// <param name="request">Comando de criacao.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        public async Task<Result<CustomerDto>> Handle(
            CreateCompanyCustomerCommand request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<CustomerDto>.Failure("Requisicao obrigatoria.");

            var validation = _companyValidator.Validate(request);
            if (!validation.IsValid)
                return Result<CustomerDto>.Failure(validation);

            try
            {
                var address = new Endereco(
                    request.Address.Street,
                    request.Address.Number,
                    request.Address.Complement,
                    request.Address.PostalCode,
                    request.Address.City,
                    request.Address.State);

                var customer = ClientePessoaJuridica.Criar(
                    request.CorporateName,
                    request.TradeName,
                    request.Cnpj,
                    request.Email,
                    request.Phone,
                    address);

                return await InsertCustomerAsync(customer, cancellationToken).ConfigureAwait(false);
            }
            catch (DomainException ex)
            {
                return Result<CustomerDto>.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Executa o comando de atualizacao de cliente pessoa fisica.
        /// </summary>
        /// <param name="request">Comando de atualizacao.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        public async Task<Result<CustomerDto>> Handle(
            UpdateIndividualCustomerCommand request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<CustomerDto>.Failure("Requisicao obrigatoria.");

            var validation = _updateIndividualValidator.Validate(request);
            if (!validation.IsValid)
                return Result<CustomerDto>.Failure(validation);

            var repository = _unitOfWork.GetRepository<Customer>();
            var customer = await repository
                .FindAsync(new object[] { request.Id }, cancellationToken)
                .ConfigureAwait(false);

            if (customer is null)
                return Result<CustomerDto>.Failure("Cliente nao encontrado.");

            if (customer is not ClientePessoaFisica individual)
                return Result<CustomerDto>.Failure("Tipo de cliente invalido.");

            try
            {
                var address = new Endereco(
                    request.Address.Street,
                    request.Address.Number,
                    request.Address.Complement,
                    request.Address.PostalCode,
                    request.Address.City,
                    request.Address.State);

                individual.Atualizar(
                    request.FullName,
                    request.Email,
                    request.Phone,
                    request.BirthDate,
                    address);

                var updateResult = await repository
                    .UpdateAsync(individual, cancellationToken)
                    .ConfigureAwait(false);

                if (!updateResult.IsSuccess)
                    return Result<CustomerDto>.Failure(updateResult.Errors, updateResult.Message);

                var enqueueResult = await EnqueueEventAsync(
                        new ClienteAtualizado(
                            individual.Id,
                            individual.TipoCliente,
                            individual.GetDocumento(),
                            individual.GetNome()),
                        cancellationToken)
                    .ConfigureAwait(false);

                if (!enqueueResult.IsSuccess)
                    return Result<CustomerDto>.Failure(enqueueResult.Errors, enqueueResult.Message);

                var commitResult = await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
                if (!commitResult.IsSuccess)
                    return Result<CustomerDto>.Failure(commitResult.Errors, commitResult.Message);

                return Result<CustomerDto>.Success(CustomerDtoMapper.Map(individual));
            }
            catch (DomainException ex)
            {
                return Result<CustomerDto>.Failure(ex.Message);
            }
        }

        /// <summary>
        /// Executa o comando de atualizacao de cliente pessoa juridica.
        /// </summary>
        /// <param name="request">Comando de atualizacao.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Resultado da operacao.</returns>
        public async Task<Result<CustomerDto>> Handle(
            UpdateCompanyCustomerCommand request,
            CancellationToken cancellationToken)
        {
            if (request is null)
                return Result<CustomerDto>.Failure("Requisicao obrigatoria.");

            var validation = _updateCompanyValidator.Validate(request);
            if (!validation.IsValid)
                return Result<CustomerDto>.Failure(validation);

            var repository = _unitOfWork.GetRepository<Customer>();
            var customer = await repository
                .FindAsync(new object[] { request.Id }, cancellationToken)
                .ConfigureAwait(false);

            if (customer is null)
                return Result<CustomerDto>.Failure("Cliente nao encontrado.");

            if (customer is not ClientePessoaJuridica company)
                return Result<CustomerDto>.Failure("Tipo de cliente invalido.");

            try
            {
                var address = new Endereco(
                    request.Address.Street,
                    request.Address.Number,
                    request.Address.Complement,
                    request.Address.PostalCode,
                    request.Address.City,
                    request.Address.State);

                company.Atualizar(
                    request.CorporateName,
                    request.TradeName,
                    request.Email,
                    request.Phone,
                    address);

                var updateResult = await repository
                    .UpdateAsync(company, cancellationToken)
                    .ConfigureAwait(false);

                if (!updateResult.IsSuccess)
                    return Result<CustomerDto>.Failure(updateResult.Errors, updateResult.Message);

                var enqueueResult = await EnqueueEventAsync(
                        new ClienteAtualizado(
                            company.Id,
                            company.TipoCliente,
                            company.GetDocumento(),
                            company.GetNome()),
                        cancellationToken)
                    .ConfigureAwait(false);

                if (!enqueueResult.IsSuccess)
                    return Result<CustomerDto>.Failure(enqueueResult.Errors, enqueueResult.Message);

                var commitResult = await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
                if (!commitResult.IsSuccess)
                    return Result<CustomerDto>.Failure(commitResult.Errors, commitResult.Message);

                return Result<CustomerDto>.Success(CustomerDtoMapper.Map(company));
            }
            catch (DomainException ex)
            {
                return Result<CustomerDto>.Failure(ex.Message);
            }
        }
        #endregion

        #region Private Methods/Operators
        private async Task<Result<CustomerDto>> InsertCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.GetRepository<Customer>();

            var documentExists = await _documentChecker
                .ExistsAsync(customer.GetDocumento(), cancellationToken)
                .ConfigureAwait(false);

            if (documentExists)
                return Result<CustomerDto>.Failure("Documento ja cadastrado.");

            var insertResult = await repository
                .InsertAsync(customer, cancellationToken)
                .ConfigureAwait(false);

            if (!insertResult.IsSuccess)
                return Result<CustomerDto>.Failure(insertResult.Errors, insertResult.Message);

            var enqueueResult = await EnqueueEventAsync(
                    new ClienteCriado(
                        customer.Id,
                        customer.TipoCliente,
                        customer.GetDocumento(),
                        customer.GetNome()),
                    cancellationToken)
                .ConfigureAwait(false);

            if (!enqueueResult.IsSuccess)
                return Result<CustomerDto>.Failure(enqueueResult.Errors, enqueueResult.Message);

            var commitResult = await _unitOfWork.CommitAsync(cancellationToken).ConfigureAwait(false);
            if (!commitResult.IsSuccess)
                return Result<CustomerDto>.Failure(commitResult.Errors, commitResult.Message);

            return Result<CustomerDto>.Success(CustomerDtoMapper.Map(customer));
        }

        private async Task<Result> EnqueueEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            if (domainEvent is null)
                throw new ArgumentNullException(nameof(domainEvent));

            try
            {
                await _outboxWriter.EnqueueAsync(domainEvent, cancellationToken).ConfigureAwait(false);
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Falha ao registrar outbox: {ex.Message}");
            }
        }
        #endregion
    }
}