using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Application.Validators;
using CustomerPlatform.UnitTests.Assets;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Validators
{
    [Trait("Application", "Validators")]
    public sealed class CreateCompanyCustomerValidatorTests
    {
        #region Variables
        private readonly CreateCompanyCustomerValidator _validator;
        #endregion

        #region SetUp Methods
        public CreateCompanyCustomerValidatorTests()
        {
            _validator = new CreateCompanyCustomerValidator();
        }
        #endregion

        #region Test Methods - Validate Valid Cases
        [Fact]
        public void Validate_ValidRequest_ShouldBeValid()
        {
            var command = CreateValidCommand();

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }
        #endregion

        #region Test Methods - Validate Invalid Cases
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_MissingCorporateName_ShouldBeInvalid(string value)
        {
            var command = CreateValidCommand(corporateName: value);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "CorporateName");
        }

        [Theory]
        [InlineData("123")]
        [InlineData("1234567890123")]
        public void Validate_InvalidCnpjLength_ShouldBeInvalid(string value)
        {
            var command = CreateValidCommand(cnpj: value);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "Cnpj");
        }

        [Fact]
        public void Validate_MissingAddressCity_ShouldBeInvalid()
        {
            var address = new AddressDto
            {
                Street = TestData.Logradouro,
                Number = TestData.NumeroEndereco,
                PostalCode = TestData.Cep,
                City = "",
                State = TestData.Estado,
            };
            var command = CreateValidCommand(address: address);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "Address.City");
        }
        #endregion

        #region Private Methods/Operators
        private static CreateCompanyCustomerCommand CreateValidCommand(
            string? corporateName = null,
            string? tradeName = null,
            string? cnpj = null,
            string? email = null,
            string? phone = null,
            AddressDto? address = null)
        {
            return new CreateCompanyCustomerCommand
            {
                CorporateName = corporateName ?? TestData.RazaoSocial,
                TradeName = tradeName ?? TestData.NomeFantasia,
                Cnpj = cnpj ?? TestData.CnpjValido,
                Email = email ?? TestData.EmailValido,
                Phone = phone ?? TestData.TelefoneValido,
                Address = address ?? new AddressDto
                {
                    Street = TestData.Logradouro,
                    Number = TestData.NumeroEndereco,
                    PostalCode = TestData.Cep,
                    City = TestData.Cidade,
                    State = TestData.Estado,
                },
            };
        }
        #endregion
    }
}
