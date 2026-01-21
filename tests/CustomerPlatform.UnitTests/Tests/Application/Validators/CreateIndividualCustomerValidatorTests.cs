using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Application.Validators;
using CustomerPlatform.UnitTests.Assets;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Validators
{
    [Trait("Application", "Validators")]
    public sealed class CreateIndividualCustomerValidatorTests
    {
        #region Variables
        private readonly CreateIndividualCustomerValidator _validator;
        #endregion

        #region SetUp Methods
        public CreateIndividualCustomerValidatorTests()
        {
            _validator = new CreateIndividualCustomerValidator();
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
        public void Validate_MissingFullName_ShouldBeInvalid(string value)
        {
            var command = CreateValidCommand(fullName: value);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
        }

        [Theory]
        [InlineData("123")]
        [InlineData("1234567890")]
        public void Validate_InvalidCpfLength_ShouldBeInvalid(string value)
        {
            var command = CreateValidCommand(cpf: value);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "Cpf");
        }

        [Fact]
        public void Validate_DefaultBirthDate_ShouldBeInvalid()
        {
            var command = CreateValidCommand(birthDate: default(DateOnly));

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "BirthDate");
        }

        [Fact]
        public void Validate_MissingAddressStreet_ShouldBeInvalid()
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
        private static CreateIndividualCustomerCommand CreateValidCommand(
            string? fullName = null,
            string? cpf = null,
            string? email = null,
            string? phone = null,
            DateOnly? birthDate = null,
            AddressDto? address = null)
        {
            return new CreateIndividualCustomerCommand
            {
                FullName = fullName ?? TestData.NomeCompleto,
                Cpf = cpf ?? TestData.CpfValido,
                Email = email ?? TestData.EmailValido,
                Phone = phone ?? TestData.TelefoneValido,
                BirthDate = birthDate ?? TestData.DataNascimento,
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
