using CustomerPlatform.Application.Abstractions.Validation;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Abstractions
{
    [Trait("Application", "Abstractions")]
    public sealed class ValidationResultTests
    {
        #region Test Methods
        [Fact]
        public void Success_DeveSerValidoESemErros()
        {
            // Act
            var result = ValidationResult.Success();

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Failure_ComErros_DeveSerInvalido()
        {
            // Arrange
            var errors = new[]
            {
                new ValidationError("Nome", "Nome obrigatorio.")
            };

            // Act
            var result = ValidationResult.Failure(errors);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Failure_ComErrosNulos_DeveLancarExcecao()
        {
            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => ValidationResult.Failure(null!));
        }
        #endregion
    }
}