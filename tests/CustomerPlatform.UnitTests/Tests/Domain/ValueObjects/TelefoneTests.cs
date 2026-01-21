using System.Collections.Generic;
using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.Domain.ValueObjects;
using CustomerPlatform.UnitTests.Assets;
using Xunit;
using Xunit.Abstractions;

namespace CustomerPlatform.UnitTests.Tests.Domain.ValueObjects
{
    [Trait("Category", "Unit")]
    public sealed class TelefoneTests
    {
        #region Variables
        private readonly ITestOutputHelper _output;
        #endregion

        #region SetUp Methods
        public TelefoneTests(ITestOutputHelper output)
        {
            _output = output;
        }
        #endregion

        #region Test Methods - Constructor Valid Cases
        [Theory]
        [MemberData(nameof(ValidTelefones))]
        public void Constructor_TelefoneValido_DeveCriarTelefone(string telefone, string esperado)
        {
            // Arrange
            var telefoneValido = telefone;

            // Act
            var telefoneValue = new Telefone(telefoneValido);

            // Assert
            Assert.Equal(esperado, telefoneValue.Numero);
            _output.WriteLine(telefoneValue.ToString());
        }
        #endregion

        #region Test Methods - Constructor Invalid Cases
        [Theory]
        [MemberData(nameof(InvalidTelefones))]
        public void Constructor_TelefoneInvalido_DeveLancarExcecao(string telefone)
        {
            // Arrange
            var telefoneInvalido = telefone;

            // Act
            var ex = Assert.Throws<InvalidValueException>(() => new Telefone(telefoneInvalido));

            // Assert
            Assert.NotNull(ex);
        }
        #endregion

        #region Test Methods - Equals Valid Cases
        [Fact]
        public void Equals_TelefonesIguais_DeveSerVerdadeiro()
        {
            // Arrange
            var telefoneA = new Telefone(TestData.TelefoneValido);
            var telefoneB = new Telefone(TestData.TelefoneValido);

            // Act
            var resultado = telefoneA == telefoneB;

            // Assert
            Assert.True(resultado);
            Assert.True(telefoneA.Equals(telefoneB));
            Assert.Equal(telefoneA.GetHashCode(), telefoneB.GetHashCode());
        }
        #endregion

        #region Test Cases
        public static IEnumerable<object[]> InvalidTelefones
        {
            get
            {
                yield return new object[] { "" };
                yield return new object[] { "1199-999" };
                yield return new object[] { "telefone" };
                yield return new object[] { "123" };
                yield return new object[] { "(11) 9999-999" };
                yield return new object[] { "119999999999" };
            }
        }

        public static IEnumerable<object[]> ValidTelefones
        {
            get
            {
                yield return new object[] { "1133334444", "1133334444" };
                yield return new object[] { "11999999999", "11999999999" };
                yield return new object[] { "(11) 99999-999", "1199999999" };
                yield return new object[] { "(11) 99999-9999", "11999999999" };
                yield return new object[] { "(11) 3333-4444", "1133334444" };
            }
        }
        #endregion
    }
}
