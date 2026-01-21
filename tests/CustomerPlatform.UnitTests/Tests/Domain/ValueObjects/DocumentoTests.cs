using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.Domain.ValueObjects;
using CustomerPlatform.UnitTests.Assets;
using Xunit;
using Xunit.Abstractions;

namespace CustomerPlatform.UnitTests.Tests.Domain.ValueObjects
{
    [Trait("Category", "Unit")]
    public sealed class DocumentoTests
    {
        #region Variables
        private readonly ITestOutputHelper _output;
        #endregion

        #region SetUp Methods
        public DocumentoTests(ITestOutputHelper output)
        {
            _output = output;
        }
        #endregion

        #region Test Methods - Constructor Valid Cases
        [Theory]
        [InlineData("178.710.184-34", "17871018434", TipoCliente.PF)]
        [InlineData("086.087.445-11", "08608744511", TipoCliente.PF)]
        [InlineData("447.131.118-21", "44713111821", TipoCliente.PF)]
        [InlineData("656.533.181-24", "65653318124", TipoCliente.PF)]
        [InlineData("024.531.412-10", "02453141210", TipoCliente.PF)]
        [InlineData("12345678909", "12345678909", TipoCliente.PF)]
        [InlineData("98765432100", "98765432100", TipoCliente.PF)]
        [InlineData("11.222.333/0001-81", "11222333000181", TipoCliente.PJ)]
        [InlineData("07.412.627/0001-07", "07412627000107", TipoCliente.PJ)]
        [InlineData("26.275.006/0001-65", "26275006000165", TipoCliente.PJ)]
        [InlineData("03.484.007/0001-14", "03484007000114", TipoCliente.PJ)]
        [InlineData("86655518000189", "86655518000189", TipoCliente.PJ)]
        [InlineData("81813242000104", "81813242000104", TipoCliente.PJ)]
        public void Constructor_DocumentoValido_DeveCriarDocumento(string valor, string esperado, TipoCliente tipoCliente)
        {
            // Arrange
            var documentoEntrada = valor;

            // Act
            var documento = new Documento(documentoEntrada);

            // Assert
            Assert.Equal(esperado, documento.Numero);
            Assert.Equal(tipoCliente, documento.TipoCliente);
            _output.WriteLine(documento.ToString());
        }
        #endregion

        #region Test Methods - Constructor Invalid Cases
        [Theory]
        [InlineData("12.345.678/0001-00")]
        [InlineData("12.345.678/0001-01")]
        [InlineData("12.345.678/0001-09")]
        [InlineData("12.345.678/0001-99")]
        [InlineData("12.345.678/0001-00a")]
        [InlineData("12.345.678/0001-100")]
        [InlineData("11.222.333/0001-80")]
        [InlineData("12.345.678/0001-10")]
        [InlineData("123.456.789-01")]
        [InlineData("98765432101")]
        [InlineData("111.222.333-55")]
        [InlineData("99999999990")]
        [InlineData("00000000001")]
        [InlineData("abc123")]
        [InlineData("123456789012345")]
        [InlineData("111.222.333-44")]
        [InlineData("99999999999")]
        [InlineData("00000000000")]
        [InlineData("")]
        public void Constructor_DocumentoInvalido_DeveLancarExcecao(string documento)
        {
            // Arrange
            var documentoInvalido = documento;

            // Act
            var ex = Assert.Throws<InvalidValueException>(() => new Documento(documentoInvalido));

            // Assert
            Assert.NotNull(ex);
        }
        #endregion

        #region Test Methods - Equals Valid Cases
        [Fact]
        public void Equals_DocumentosIguais_DeveSerVerdadeiro()
        {
            // Arrange
            var documentoA = new Documento(TestData.CpfValido);
            var documentoB = new Documento(TestData.CpfValido);

            // Act
            var resultado = documentoA == documentoB;

            // Assert
            Assert.True(resultado);
            Assert.True(documentoA.Equals(documentoB));
            Assert.Equal(documentoA.GetHashCode(), documentoB.GetHashCode());
        }
        #endregion
    }
}
