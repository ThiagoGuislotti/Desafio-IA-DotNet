using CustomerPlatform.Application.Abstractions;
using CustomerPlatform.Application.Abstractions.Messaging;
using CustomerPlatform.Application.Abstractions.Repositories;
using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Validation;
using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.Cqrs.Commands.Handlers;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Entities;
using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Events;
using CustomerPlatform.Domain.ValueObjects;
using CustomerPlatform.UnitTests.Assets;
using Moq;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Commands
{
    [Trait("Application", "Commands")]
    public sealed class CustomerCommandHandlerTests
    {
        #region Variables
        private readonly Mock<IRepository<Customer>> _repository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ICustomerDocumentChecker> _documentChecker;
        private readonly Mock<IOutboxWriter> _outboxWriter;
        private readonly Mock<IValidator<CreateIndividualCustomerCommand>> _individualValidator;
        private readonly Mock<IValidator<CreateCompanyCustomerCommand>> _companyValidator;
        private readonly Mock<IValidator<UpdateIndividualCustomerCommand>> _updateIndividualValidator;
        private readonly Mock<IValidator<UpdateCompanyCustomerCommand>> _updateCompanyValidator;
        private readonly CustomerCommandHandler _handler;
        #endregion

        #region SetUp Methods
        public CustomerCommandHandlerTests()
        {
            _repository = new Mock<IRepository<Customer>>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _documentChecker = new Mock<ICustomerDocumentChecker>();
            _outboxWriter = new Mock<IOutboxWriter>();
            _individualValidator = new Mock<IValidator<CreateIndividualCustomerCommand>>();
            _companyValidator = new Mock<IValidator<CreateCompanyCustomerCommand>>();
            _updateIndividualValidator = new Mock<IValidator<UpdateIndividualCustomerCommand>>();
            _updateCompanyValidator = new Mock<IValidator<UpdateCompanyCustomerCommand>>();

            _unitOfWork.Setup(unitOfWork => unitOfWork.GetRepository<Customer>())
                .Returns(_repository.Object);
            _outboxWriter
                .Setup(writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _documentChecker
                .Setup(checker => checker.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _handler = new CustomerCommandHandler(
                _unitOfWork.Object,
                _documentChecker.Object,
                _outboxWriter.Object,
                _individualValidator.Object,
                _companyValidator.Object,
                _updateIndividualValidator.Object,
                _updateCompanyValidator.Object);
        }
        #endregion

        #region Test Methods - Handle CreateIndividualCustomerCommand Valid Cases
        [Fact]
        public async Task Handle_CreateIndividualCustomerCommand_ValidRequest_ShouldCreateCustomer()
        {
            var command = CreateValidIndividualCommand();

            _individualValidator
                .Setup(validator => validator.Validate(It.IsAny<CreateIndividualCustomerCommand>()))
                .Returns(ValidationResult.Success());
            _repository
                .Setup(repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());
            _unitOfWork
                .Setup(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(TestData.CpfValido, result.Data!.Document);
            Assert.Equal(TipoCliente.PF, result.Data.CustomerType);
            _repository.Verify(
                repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _outboxWriter.Verify(
                writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWork.Verify(
                unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
        #endregion

        #region Test Methods - Handle CreateIndividualCustomerCommand Invalid Cases
        [Fact]
        public async Task Handle_CreateIndividualCustomerCommand_InvalidRequest_ShouldReturnFailure()
        {
            var command = CreateValidIndividualCommand();

            _individualValidator
                .Setup(validator => validator.Validate(It.IsAny<CreateIndividualCustomerCommand>()))
                .Returns(ValidationResult.Failure(
                    new[] { new ValidationError("FullName", "Campo obrigatorio.") }));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _repository.Verify(
                repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _outboxWriter.Verify(
                writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [InlineData("11111111111")]
        public async Task Handle_CreateIndividualCustomerCommand_InvalidDomain_ShouldReturnFailure(string cpf)
        {
            var command = CreateValidIndividualCommand(cpf: cpf);

            _individualValidator
                .Setup(validator => validator.Validate(It.IsAny<CreateIndividualCustomerCommand>()))
                .Returns(ValidationResult.Success());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _repository.Verify(
                repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _outboxWriter.Verify(
                writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        #endregion

        #region Test Methods - Handle CreateIndividualCustomerCommand Exception Cases
        [Fact]
        public async Task Handle_CreateIndividualCustomerCommand_OutboxFailure_ShouldReturnFailure()
        {
            var command = CreateValidIndividualCommand();

            _individualValidator
                .Setup(validator => validator.Validate(It.IsAny<CreateIndividualCustomerCommand>()))
                .Returns(ValidationResult.Success());
            _repository
                .Setup(repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());
            _outboxWriter
                .Setup(writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Falha outbox"));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            _outboxWriter.Verify(
                writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWork.Verify(
                unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }
        #endregion

        #region Test Methods - Handle CreateCompanyCustomerCommand Valid Cases
        [Fact]
        public async Task Handle_CreateCompanyCustomerCommand_ValidRequest_ShouldCreateCustomer()
        {
            var command = CreateValidCompanyCommand();

            _companyValidator
                .Setup(validator => validator.Validate(It.IsAny<CreateCompanyCustomerCommand>()))
                .Returns(ValidationResult.Success());
            _repository
                .Setup(repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());
            _unitOfWork
                .Setup(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(TestData.CnpjValido, result.Data!.Document);
            Assert.Equal(TipoCliente.PJ, result.Data.CustomerType);
            _repository.Verify(
                repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _outboxWriter.Verify(
                writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _unitOfWork.Verify(
                unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
        #endregion

        #region Test Methods - Handle CreateCompanyCustomerCommand Invalid Cases
        [Fact]
        public async Task Handle_CreateCompanyCustomerCommand_InvalidRequest_ShouldReturnFailure()
        {
            var command = CreateValidCompanyCommand();

            _companyValidator
                .Setup(validator => validator.Validate(It.IsAny<CreateCompanyCustomerCommand>()))
                .Returns(ValidationResult.Failure(
                    new[] { new ValidationError("CorporateName", "Campo obrigatorio.") }));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _repository.Verify(
                repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _outboxWriter.Verify(
                writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [InlineData("86655518000180")]
        public async Task Handle_CreateCompanyCustomerCommand_InvalidDomain_ShouldReturnFailure(string cnpj)
        {
            var command = CreateValidCompanyCommand(cnpj: cnpj);

            _companyValidator
                .Setup(validator => validator.Validate(It.IsAny<CreateCompanyCustomerCommand>()))
                .Returns(ValidationResult.Success());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _repository.Verify(
                repository => repository.InsertAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Never);
            _outboxWriter.Verify(
                writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        #endregion

        #region Test Methods - Handle UpdateIndividualCustomerCommand Valid Cases
        [Fact]
        public async Task Handle_UpdateIndividualCustomerCommand_ValidRequest_ShouldUpdateCustomer()
        {
            var customer = CreateIndividualCustomer();
            var command = CreateValidUpdateIndividualCommand(customer.Id);

            _updateIndividualValidator
                .Setup(validator => validator.Validate(It.IsAny<UpdateIndividualCustomerCommand>()))
                .Returns(ValidationResult.Success());
            _repository
                .Setup(repository => repository.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);
            _repository
                .Setup(repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());
            _unitOfWork
                .Setup(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(customer.Id, result.Data!.Id);
            _repository.Verify(
                repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _outboxWriter.Verify(
                writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        #endregion

        #region Test Methods - Handle UpdateIndividualCustomerCommand Invalid Cases
        [Fact]
        public async Task Handle_UpdateIndividualCustomerCommand_InvalidRequest_ShouldReturnFailure()
        {
            var command = CreateValidUpdateIndividualCommand(Guid.NewGuid());

            _updateIndividualValidator
                .Setup(validator => validator.Validate(It.IsAny<UpdateIndividualCustomerCommand>()))
                .Returns(ValidationResult.Failure(
                    new[] { new ValidationError("FullName", "Campo obrigatorio.") }));

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _repository.Verify(
                repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateIndividualCustomerCommand_NotFound_ShouldReturnFailure()
        {
            var command = CreateValidUpdateIndividualCommand(Guid.NewGuid());

            _updateIndividualValidator
                .Setup(validator => validator.Validate(It.IsAny<UpdateIndividualCustomerCommand>()))
                .Returns(ValidationResult.Success());
            _repository
                .Setup(repository => repository.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _repository.Verify(
                repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateIndividualCustomerCommand_WrongType_ShouldReturnFailure()
        {
            var customer = CreateCompanyCustomer();
            var command = CreateValidUpdateIndividualCommand(customer.Id);

            _updateIndividualValidator
                .Setup(validator => validator.Validate(It.IsAny<UpdateIndividualCustomerCommand>()))
                .Returns(ValidationResult.Success());
            _repository
                .Setup(repository => repository.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _repository.Verify(
                repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Theory]
        [InlineData("email-invalido")]
        public async Task Handle_UpdateIndividualCustomerCommand_InvalidDomain_ShouldReturnFailure(string email)
        {
            var customer = CreateIndividualCustomer();
            var command = CreateValidUpdateIndividualCommand(customer.Id, email: email);

            _updateIndividualValidator
                .Setup(validator => validator.Validate(It.IsAny<UpdateIndividualCustomerCommand>()))
                .Returns(ValidationResult.Success());
            _repository
                .Setup(repository => repository.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _repository.Verify(
                repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        #endregion

        #region Test Methods - Handle UpdateCompanyCustomerCommand Valid Cases
        [Fact]
        public async Task Handle_UpdateCompanyCustomerCommand_ValidRequest_ShouldUpdateCustomer()
        {
            var customer = CreateCompanyCustomer();
            var command = CreateValidUpdateCompanyCommand(customer.Id);

            _updateCompanyValidator
                .Setup(validator => validator.Validate(It.IsAny<UpdateCompanyCustomerCommand>()))
                .Returns(ValidationResult.Success());
            _repository
                .Setup(repository => repository.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);
            _repository
                .Setup(repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());
            _unitOfWork
                .Setup(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(customer.Id, result.Data!.Id);
            _repository.Verify(
                repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Once);
            _outboxWriter.Verify(
                writer => writer.EnqueueAsync(It.IsAny<IDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        #endregion

        #region Test Methods - Handle UpdateCompanyCustomerCommand Invalid Cases
        [Fact]
        public async Task Handle_UpdateCompanyCustomerCommand_WrongType_ShouldReturnFailure()
        {
            var customer = CreateIndividualCustomer();
            var command = CreateValidUpdateCompanyCommand(customer.Id);

            _updateCompanyValidator
                .Setup(validator => validator.Validate(It.IsAny<UpdateCompanyCustomerCommand>()))
                .Returns(ValidationResult.Success());
            _repository
                .Setup(repository => repository.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _repository.Verify(
                repository => repository.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
        #endregion

        #region Private Methods/Operators
        private static CreateIndividualCustomerCommand CreateValidIndividualCommand(
            string? fullName = null,
            string? cpf = null,
            string? email = null,
            string? phone = null,
            DateOnly? birthDate = null)
        {
            return new CreateIndividualCustomerCommand
            {
                FullName = fullName ?? TestData.NomeCompleto,
                Cpf = cpf ?? TestData.CpfValido,
                Email = email ?? TestData.EmailValido,
                Phone = phone ?? TestData.TelefoneValido,
                BirthDate = birthDate ?? TestData.DataNascimento,
                Address = new AddressDto
                {
                    Street = TestData.Logradouro,
                    Number = TestData.NumeroEndereco,
                    PostalCode = TestData.Cep,
                    City = TestData.Cidade,
                    State = TestData.Estado,
                },
            };
        }

        private static CreateCompanyCustomerCommand CreateValidCompanyCommand(
            string? corporateName = null,
            string? tradeName = null,
            string? cnpj = null,
            string? email = null,
            string? phone = null)
        {
            return new CreateCompanyCustomerCommand
            {
                CorporateName = corporateName ?? TestData.RazaoSocial,
                TradeName = tradeName ?? TestData.NomeFantasia,
                Cnpj = cnpj ?? TestData.CnpjValido,
                Email = email ?? TestData.EmailValido,
                Phone = phone ?? TestData.TelefoneValido,
                Address = new AddressDto
                {
                    Street = TestData.Logradouro,
                    Number = TestData.NumeroEndereco,
                    PostalCode = TestData.Cep,
                    City = TestData.Cidade,
                    State = TestData.Estado,
                },
            };
        }

        private static UpdateIndividualCustomerCommand CreateValidUpdateIndividualCommand(
            Guid id,
            string? fullName = null,
            string? email = null,
            string? phone = null,
            DateOnly? birthDate = null)
        {
            return new UpdateIndividualCustomerCommand
            {
                Id = id,
                FullName = fullName ?? TestData.NomeCompleto,
                Email = email ?? TestData.EmailValido,
                Phone = phone ?? TestData.TelefoneValido,
                BirthDate = birthDate ?? TestData.DataNascimento,
                Address = new AddressDto
                {
                    Street = TestData.Logradouro,
                    Number = TestData.NumeroEndereco,
                    PostalCode = TestData.Cep,
                    City = TestData.Cidade,
                    State = TestData.Estado,
                },
            };
        }

        private static UpdateCompanyCustomerCommand CreateValidUpdateCompanyCommand(
            Guid id,
            string? corporateName = null,
            string? tradeName = null,
            string? email = null,
            string? phone = null)
        {
            return new UpdateCompanyCustomerCommand
            {
                Id = id,
                CorporateName = corporateName ?? TestData.RazaoSocial,
                TradeName = tradeName ?? TestData.NomeFantasia,
                Email = email ?? TestData.EmailValido,
                Phone = phone ?? TestData.TelefoneValido,
                Address = new AddressDto
                {
                    Street = TestData.Logradouro,
                    Number = TestData.NumeroEndereco,
                    PostalCode = TestData.Cep,
                    City = TestData.Cidade,
                    State = TestData.Estado,
                },
            };
        }

        private static Customer CreateIndividualCustomer()
        {
            var address = new Endereco(
                TestData.Logradouro,
                TestData.NumeroEndereco,
                null,
                TestData.Cep,
                TestData.Cidade,
                TestData.Estado);

            return ClientePessoaFisica.Criar(
                TestData.NomeCompleto,
                TestData.CpfValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                TestData.DataNascimento,
                address);
        }

        private static Customer CreateCompanyCustomer()
        {
            var address = new Endereco(
                TestData.Logradouro,
                TestData.NumeroEndereco,
                null,
                TestData.Cep,
                TestData.Cidade,
                TestData.Estado);

            return ClientePessoaJuridica.Criar(
                TestData.RazaoSocial,
                TestData.NomeFantasia,
                TestData.CnpjValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                address);
        }
        #endregion
    }
}
