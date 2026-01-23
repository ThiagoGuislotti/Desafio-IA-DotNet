using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Validation;
using Xunit;

namespace CustomerPlatform.UnitTests.Tests.Application.Abstractions
{
    [Trait("Application", "Abstractions")]
    public sealed class ResultTests
    {
        #region Test Methods - Result
        [Fact]
        public void Success_DeveRetornarSucessoESemErros()
        {
            // Arrange
            const string message = "ok";

            // Act
            var result = Result.Success(message);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
            Assert.Equal(message, result.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Failure_ComMensagemVazia_DeveLancarExcecao(string? message)
        {
            // Act + Assert
            Assert.Throws<ArgumentException>(() => Result.Failure(message!));
        }

        [Fact]
        public void Failure_ComValidationResultNulo_DeveLancarExcecao()
        {
            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => Result.Failure((ValidationResult)null!));
        }

        [Fact]
        public void Failure_ComErrosNulos_DeveLancarExcecao()
        {
            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => Result.Failure((IEnumerable<ValidationError>)null!));
        }
        #endregion

        #region Test Methods - Result Generic
        [Fact]
        public void ResultGeneric_Success_DeveRetornarDados()
        {
            // Arrange
            const string data = "valor";

            // Act
            var result = Result<string>.Success(data, "ok");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(data, result.Data);
        }

        [Fact]
        public void ResultGeneric_Success_ComDadosNulos_DeveLancarExcecao()
        {
            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => Result<string>.Success(null!));
        }

        [Fact]
        public void ResultGeneric_Failure_ComMensagemVazia_DeveLancarExcecao()
        {
            // Act + Assert
            Assert.Throws<ArgumentException>(() => Result<string>.Failure(" "));
        }

        [Fact]
        public void ResultGeneric_Failure_ComValidationResultNulo_DeveLancarExcecao()
        {
            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => Result<string>.Failure((ValidationResult)null!));
        }

        [Fact]
        public void ResultGeneric_Failure_ComErrosNulos_DeveLancarExcecao()
        {
            // Act + Assert
            Assert.Throws<ArgumentNullException>(() => Result<string>.Failure((IEnumerable<ValidationError>)null!));
        }
        #endregion
    }
}