using System;
using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Events;
using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.UnitTests.Assets;
using Xunit;
using Xunit.Abstractions;

namespace CustomerPlatform.UnitTests.Tests.Domain.Events
{
    [Trait("Category", "Unit")]
    public sealed class ClienteAtualizadoTests
    {
        #region Variables
        private readonly ITestOutputHelper _output;
        #endregion

        #region SetUp Methods
        public ClienteAtualizadoTests(ITestOutputHelper output)
        {
            _output = output;
        }
        #endregion

        #region Test Methods - Constructor Valid Cases
        [Fact]
        public void Constructor_DadosValidos_DeveCriarEvento()
        {
            // Arrange
            var clienteId = Guid.NewGuid();

            // Act
            var evento = new ClienteAtualizado(
                clienteId,
                TipoCliente.PF,
                TestData.CpfValido,
                TestData.NomeCompleto);

            // Assert
            Assert.Equal(clienteId, evento.ClienteId);
            Assert.Equal(TipoCliente.PF, evento.TipoCliente);
            Assert.Equal(TestData.CpfValido, evento.Documento);
            Assert.Equal(TestData.NomeCompleto, evento.Nome);
            Assert.Equal(ClienteAtualizado.EventTypeName, evento.EventType);
            Assert.NotEqual(Guid.Empty, evento.EventId);
            _output.WriteLine(evento.EventType);
        }
        #endregion

        #region Test Methods - Constructor Invalid Cases
        [Fact]
        public void Constructor_ClienteIdVazio_DeveLancarExcecao()
        {
            // Arrange
            var clienteId = Guid.Empty;

            // Act
            var ex = Assert.Throws<RequiredFieldException>(() => new ClienteAtualizado(
                clienteId,
                TipoCliente.PF,
                TestData.CpfValido,
                TestData.NomeCompleto));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Constructor_DocumentoVazio_DeveLancarExcecao()
        {
            // Arrange
            var documento = string.Empty;

            // Act
            var ex = Assert.Throws<RequiredFieldException>(() => new ClienteAtualizado(
                Guid.NewGuid(),
                TipoCliente.PF,
                documento,
                TestData.NomeCompleto));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Constructor_NomeVazio_DeveLancarExcecao()
        {
            // Arrange
            var nome = string.Empty;

            // Act
            var ex = Assert.Throws<RequiredFieldException>(() => new ClienteAtualizado(
                Guid.NewGuid(),
                TipoCliente.PF,
                TestData.CpfValido,
                nome));

            // Assert
            Assert.NotNull(ex);
        }
        #endregion
    }
}
