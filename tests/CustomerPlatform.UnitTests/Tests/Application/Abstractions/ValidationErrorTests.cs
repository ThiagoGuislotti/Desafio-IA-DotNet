using CustomerPlatform.Application.Abstractions.Validation;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Abstractions
{
    [Trait("Application", "Abstractions")]
    public sealed class ValidationErrorTests
    {
        #region Test Methods
        [Fact]
        public void Construtor_DadosValidos_DevePreencherPropriedades()
        {
            // Arrange
            const string propertyName = "Email";
            const string errorMessage = "Email obrigatorio.";

            // Act
            var error = new ValidationError(propertyName, errorMessage);

            // Assert
            Assert.Equal(propertyName, error.PropertyName);
            Assert.Equal(errorMessage, error.ErrorMessage);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_PropertyNameVazio_DeveLancarExcecao(string? propertyName)
        {
            // Act + Assert
            Assert.Throws<ArgumentException>(() => new ValidationError(propertyName!, "Erro"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ErrorMessageVazio_DeveLancarExcecao(string? errorMessage)
        {
            // Act + Assert
            Assert.Throws<ArgumentException>(() => new ValidationError("Campo", errorMessage!));
        }
        #endregion
    }
}