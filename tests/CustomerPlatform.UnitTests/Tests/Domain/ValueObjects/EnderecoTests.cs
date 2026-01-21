using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.Domain.ValueObjects;
using CustomerPlatform.UnitTests.Assets;
using Xunit;
using Xunit.Abstractions;

namespace CustomerPlatform.UnitTests.Tests.Domain.ValueObjects
{
    [Trait("Category", "Unit")]
    public sealed class EnderecoTests
    {
        #region Variables
        private readonly ITestOutputHelper _output;
        #endregion

        #region SetUp Methods
        public EnderecoTests(ITestOutputHelper output)
        {
            _output = output;
        }
        #endregion

        #region Test Methods - Constructor Valid Cases
        [Fact]
        public void Constructor_EnderecoValido_DeveCriarEndereco()
        {
            // Arrange
            var endereco = EnderecoFactory.CriarValido();

            // Act
            var resultado = endereco;

            // Assert
            Assert.Equal(TestData.Logradouro, resultado.Logradouro);
            Assert.Equal(TestData.NumeroEndereco, resultado.Numero);
            Assert.Equal(TestData.Cep, resultado.Cep);
            Assert.Equal(TestData.Cidade, resultado.Cidade);
            Assert.Equal(TestData.Estado, resultado.Estado);
            _output.WriteLine(resultado.ToString());
        }
        #endregion

        #region Test Methods - Constructor Invalid Cases
        [Theory]
        [InlineData("", TestData.NumeroEndereco, null, TestData.Cep, TestData.Cidade, TestData.Estado)]
        [InlineData(TestData.Logradouro, "", null, TestData.Cep, TestData.Cidade, TestData.Estado)]
        [InlineData(TestData.Logradouro, TestData.NumeroEndereco, null, "", TestData.Cidade, TestData.Estado)]
        [InlineData(TestData.Logradouro, TestData.NumeroEndereco, null, TestData.Cep, "", TestData.Estado)]
        [InlineData(TestData.Logradouro, TestData.NumeroEndereco, null, TestData.Cep, TestData.Cidade, "")]
        public void Constructor_EnderecoInvalido_DeveLancarExcecao(
            string logradouro,
            string numero,
            string? complemento,
            string cep,
            string cidade,
            string estado)
        {
            // Arrange
            var logradouroInvalido = logradouro;

            // Act
            var ex = Assert.Throws<InvalidValueException>(() => new Endereco(
                logradouroInvalido,
                numero,
                complemento,
                cep,
                cidade,
                estado));

            // Assert
            Assert.NotNull(ex);
        }
        #endregion

        #region Test Methods - Equals Valid Cases
        [Fact]
        public void Equals_EnderecosIguais_DeveSerVerdadeiro()
        {
            // Arrange
            var enderecoA = EnderecoFactory.CriarValido();
            var enderecoB = EnderecoFactory.CriarValido();

            // Act
            var resultado = enderecoA == enderecoB;

            // Assert
            Assert.True(resultado);
            Assert.True(enderecoA.Equals(enderecoB));
            Assert.Equal(enderecoA.GetHashCode(), enderecoB.GetHashCode());
        }
        #endregion
    }
}
