using System;
using CustomerPlatform.Domain.Entities;
using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.Domain.ValueObjects;
using CustomerPlatform.UnitTests.Assets;
using Xunit;
using Xunit.Abstractions;

namespace CustomerPlatform.UnitTests.Tests.Domain.Entities
{
    [Trait("Domain", "Entities")]
    public sealed class ClientePessoaJuridicaTests
    {
        #region Variables
        private readonly ITestOutputHelper _output;
        private readonly Endereco _enderecoValido;
        #endregion

        #region SetUp Methods
        public ClientePessoaJuridicaTests(ITestOutputHelper output)
        {
            _output = output;
            _enderecoValido = EnderecoFactory.CriarValido();
        }
        #endregion

        #region Test Methods - Criar Valid Cases
        [Fact]
        public void Criar_DadosValidos_DeveCriarCliente()
        {
            // Arrange
            // Act
            var cliente = ClientePessoaJuridica.Criar(
                TestData.RazaoSocial,
                TestData.NomeFantasia,
                TestData.CnpjValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                _enderecoValido);

            // Assert
            Assert.NotEqual(Guid.Empty, cliente.Id);
            Assert.Equal(TestData.RazaoSocial, cliente.RazaoSocial);
            Assert.Equal(TestData.NomeFantasia, cliente.NomeFantasia);
            Assert.Equal(TestData.CnpjValido, cliente.CNPJ);
            Assert.Equal(TestData.EmailValido, cliente.Email.Endereco);
            Assert.Equal(TestData.TelefoneValido, cliente.Telefone.Numero);
            Assert.Equal(TipoCliente.PJ, cliente.TipoCliente);
            Assert.Equal(TestData.CnpjValido, cliente.GetDocumento());
            Assert.Equal(TestData.RazaoSocial, cliente.GetNome());
            _output.WriteLine(cliente.GetDocumento());
        }
        #endregion

        #region Test Methods - Criar Invalid Cases
        [Theory]
        [InlineData("123")]
        [InlineData("12.345.678/0001-00")]
        [InlineData("12.345.678/0001-00a")]
        [InlineData("11.222.333/0001-80")]
        public void Criar_CnpjInvalido_DeveLancarExcecao(string cnpjInvalido)
        {
            // Arrange
            var cnpjInvalidoValor = cnpjInvalido;

            // Act
            var ex = Assert.Throws<InvalidValueException>(() => ClientePessoaJuridica.Criar(
                TestData.RazaoSocial,
                TestData.NomeFantasia,
                cnpjInvalidoValor,
                TestData.EmailValido,
                TestData.TelefoneValido,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Criar_RazaoSocialVazia_DeveLancarExcecao()
        {
            // Arrange
            var razaoSocialInvalida = string.Empty;

            // Act
            var ex = Assert.Throws<RequiredFieldException>(() => ClientePessoaJuridica.Criar(
                razaoSocialInvalida,
                TestData.NomeFantasia,
                TestData.CnpjValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Criar_NomeFantasiaVazio_DeveLancarExcecao()
        {
            // Arrange
            var nomeFantasiaInvalido = string.Empty;

            // Act
            var ex = Assert.Throws<RequiredFieldException>(() => ClientePessoaJuridica.Criar(
                TestData.RazaoSocial,
                nomeFantasiaInvalido,
                TestData.CnpjValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }
        #endregion
    }
}
