using CustomerPlatform.Application.Cqrs.Queries;
using CustomerPlatform.Application.Validators;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Validators
{
    [Trait("Application", "Validators")]
    public sealed class GetCustomerByIdQueryValidatorTests
    {
        #region Variables
        private readonly GetCustomerByIdQueryValidator _validator;
        #endregion

        #region SetUp Methods
        public GetCustomerByIdQueryValidatorTests()
        {
            _validator = new GetCustomerByIdQueryValidator();
        }
        #endregion

        #region Test Methods - Validate Valid Cases
        [Fact]
        public void Validar_RequisicaoValida_DeveSerValida()
        {
            var query = new GetCustomerByIdQuery { Id = Guid.NewGuid() };

            var result = _validator.Validate(query);

            Assert.True(result.IsValid);
        }
        #endregion

        #region Test Methods - Validate Invalid Cases
        [Fact]
        public void Validar_IdVazio_DeveSerInvalida()
        {
            var query = new GetCustomerByIdQuery { Id = Guid.Empty };

            var result = _validator.Validate(query);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "Id");
        }
        #endregion
    }
}