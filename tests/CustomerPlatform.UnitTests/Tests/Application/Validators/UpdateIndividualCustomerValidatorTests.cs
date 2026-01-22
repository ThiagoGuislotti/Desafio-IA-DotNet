using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Application.Validators;
using CustomerPlatform.UnitTests.Assets;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Validators
{
    [Trait("Application", "Validators")]
    public sealed class UpdateIndividualCustomerValidatorTests
    {
        #region Variables
        private readonly UpdateIndividualCustomerValidator _validator;
        #endregion

        #region SetUp Methods
        public UpdateIndividualCustomerValidatorTests()
        {
            _validator = new UpdateIndividualCustomerValidator();
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
        public void Validar_NomeCompletoAusente_DeveSerInvalida(string value)
        {
            var command = CreateValidCommand(fullName: value);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
        }

        [Fact]
        public void Validar_DataNascimentoPadrao_DeveSerInvalida()
        {
            var command = CreateValidCommand(birthDate: default(DateOnly));

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.PropertyName == "BirthDate");
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
        private static UpdateIndividualCustomerCommand CreateValidCommand(
            Guid? id = null,
            string? fullName = null,
            string? email = null,
            string? phone = null,
            DateOnly? birthDate = null,
            AddressDto? address = null)
        {
            return new UpdateIndividualCustomerCommand
            {
                Id = id ?? Guid.NewGuid(),
                FullName = fullName ?? TestData.NomeCompleto,
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
