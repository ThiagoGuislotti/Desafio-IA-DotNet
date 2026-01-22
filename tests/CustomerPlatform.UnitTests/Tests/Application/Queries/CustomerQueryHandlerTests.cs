using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Application.Abstractions.Validation;
using CustomerPlatform.Application.Cqrs.Queries;
using CustomerPlatform.Application.Cqrs.Queries.Handlers;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Entities;
using CustomerPlatform.Domain.ValueObjects;
using CustomerPlatform.UnitTests.Assets;
using Moq;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Queries
{
    [Trait("Application", "Queries")]
    public sealed class CustomerQueryHandlerTests
    {
        #region Variables
        private readonly Mock<ICustomerSearchService> _searchService;
        private readonly Mock<IValidator<GetCustomerByIdQuery>> _getByIdValidator;
        private readonly Mock<IValidator<SearchCustomersQuery>> _searchValidator;
        private readonly CustomerQueryHandler _handler;
        #endregion

        #region SetUp Methods
        public CustomerQueryHandlerTests()
        {
            _searchService = new Mock<ICustomerSearchService>();
            _getByIdValidator = new Mock<IValidator<GetCustomerByIdQuery>>();
            _searchValidator = new Mock<IValidator<SearchCustomersQuery>>();

            _handler = new CustomerQueryHandler(
                _searchService.Object,
                _getByIdValidator.Object,
                _searchValidator.Object);
        }
        #endregion

        #region Test Methods - Handle GetCustomerByIdQuery Valid Cases
        [Fact]
        public async Task Tratar_GetCustomerByIdQuery_ClienteExistente_DeveRetornarDados()
        {
            var customer = CreateCustomer();
            var query = new GetCustomerByIdQuery { Id = customer.Id };
            var dto = CustomerDtoMapper.Map(customer);

            _getByIdValidator
                .Setup(validator => validator.Validate(It.IsAny<GetCustomerByIdQuery>()))
                .Returns(ValidationResult.Success());
            _searchService
                .Setup(service => service.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<CustomerDto>.Success(dto));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(customer.Id, result.Data!.Id);
        }
        #endregion

        #region Test Methods - Handle GetCustomerByIdQuery Invalid Cases
        [Fact]
        public async Task Tratar_GetCustomerByIdQuery_RequisicaoInvalida_DeveRetornarFalha()
        {
            var query = new GetCustomerByIdQuery { Id = Guid.Empty };

            _getByIdValidator
                .Setup(validator => validator.Validate(It.IsAny<GetCustomerByIdQuery>()))
                .Returns(ValidationResult.Failure(
                    new[] { new ValidationError("Id", "Id obrigatorio.") }));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task Tratar_GetCustomerByIdQuery_NaoEncontrado_DeveRetornarFalha()
        {
            var query = new GetCustomerByIdQuery { Id = Guid.NewGuid() };

            _getByIdValidator
                .Setup(validator => validator.Validate(It.IsAny<GetCustomerByIdQuery>()))
                .Returns(ValidationResult.Success());
            _searchService
                .Setup(service => service.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<CustomerDto>.Failure("Cliente nao encontrado."));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
        }
        #endregion

        #region Test Methods - Handle SearchCustomersQuery Valid Cases
        [Fact]
        public async Task Tratar_SearchCustomersQuery_ConsultaValida_DeveRetornarResultados()
        {
            var query = new SearchCustomersQuery { Name = TestData.NomeCompleto };
            var customers = new List<CustomerDto> { CustomerDtoMapper.Map(CreateCustomer()) };

            _searchValidator
                .Setup(validator => validator.Validate(It.IsAny<SearchCustomersQuery>()))
                .Returns(ValidationResult.Success());
            _searchService
                .Setup(service => service.SearchAsync(It.IsAny<CustomerSearchCriteria>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<IReadOnlyCollection<CustomerDto>>.Success(customers));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data!);
        }
        #endregion

        #region Test Methods - Handle SearchCustomersQuery Invalid Cases
        [Fact]
        public async Task Tratar_SearchCustomersQuery_ConsultaInvalida_DeveRetornarFalha()
        {
            var query = new SearchCustomersQuery { PageNumber = 0 };

            _searchValidator
                .Setup(validator => validator.Validate(It.IsAny<SearchCustomersQuery>()))
                .Returns(ValidationResult.Failure(
                    new[] { new ValidationError("PageNumber", "Numero da pagina deve ser maior que zero.") }));

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            _searchService.Verify(
                service => service.SearchAsync(
                    It.IsAny<CustomerSearchCriteria>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
        #endregion

        #region Private Methods/Operators
        private static Customer CreateCustomer()
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
        #endregion
    }
}
