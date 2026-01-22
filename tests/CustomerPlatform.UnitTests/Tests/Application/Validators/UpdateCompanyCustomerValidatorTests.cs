using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Application.Validators;
using CustomerPlatform.UnitTests.Assets;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Validators
{
    [Trait("Application", "Validators")]
    public sealed class UpdateCompanyCustomerValidatorTests
    {
        #region Variables
        private readonly UpdateCompanyCustomerValidator _validator;
        #endregion

        #region SetUp Methods
        public UpdateCompanyCustomerValidatorTests()
        {
            _validator = new UpdateCompanyCustomerValidator();
        }
        #endregion

        #region Test Methods - Validate Valid Cases
        [Fact]
        public void Validar_RequisicaoValida_DeveSerValida()
        {
            var command = CreateValidCommand();

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }
        #endregion

        #region Test Methods - Validate Invalid Cases
        [Fact]
        public void Validar_IdVazio_DeveSerInvalida()
        {
            var command = CreateValidCommand(id: Guid.Empty);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "Id");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Validar_RazaoSocialAusente_DeveSerInvalida(string value)
        {
            var command = CreateValidCommand(corporateName: value);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "CorporateName");
        }

        [Fact]
        public void Validar_LogradouroAusente_DeveSerInvalida()
        {
            var address = new AddressDto
            {
                Street = "",
                Number = TestData.NumeroEndereco,
                PostalCode = TestData.Cep,
                City = TestData.Cidade,
                State = TestData.Estado,
            };
            var command = CreateValidCommand(address: address);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "Address.Street");
        }
        #endregion

        #region Private Methods/Operators
        private static UpdateCompanyCustomerCommand CreateValidCommand(
            Guid? id = null,
            string? corporateName = null,
            string? tradeName = null,
            string? email = null,
            string? phone = null,
            AddressDto? address = null)
        {
            return new UpdateCompanyCustomerCommand
            {
                Id = id ?? Guid.NewGuid(),
                CorporateName = corporateName ?? TestData.RazaoSocial,
                TradeName = tradeName ?? TestData.NomeFantasia,
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