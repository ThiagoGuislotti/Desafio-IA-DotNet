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
    public sealed class ClientePessoaFisicaTests
    {
        #region Variables
        private readonly ITestOutputHelper _output;
        private readonly Endereco _enderecoValido;
        #endregion

        #region SetUp Methods
        public ClientePessoaFisicaTests(ITestOutputHelper output)
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
            var cliente = ClientePessoaFisica.Criar(
                TestData.NomeCompleto,
                TestData.CpfValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                TestData.DataNascimento,
                _enderecoValido);

            // Assert
            Assert.NotEqual(Guid.Empty, cliente.Id);
            Assert.Equal(TestData.NomeCompleto, cliente.Nome);
            Assert.Equal(TestData.CpfValido, cliente.CPF);
            Assert.Equal(TestData.EmailValido, cliente.Email.Endereco);
            Assert.Equal(TestData.TelefoneValido, cliente.Telefone.Numero);
            Assert.Equal(TipoCliente.PF, cliente.TipoCliente);
            Assert.Equal(TestData.CpfValido, cliente.GetDocumento());
            Assert.Equal(TestData.NomeCompleto, cliente.GetNome());
            _output.WriteLine(cliente.GetDocumento());
        }
        #endregion

        #region Test Methods - Criar Invalid Cases
        [Theory]
        [InlineData("123")]
        [InlineData("111.222.333-55")]
        [InlineData("00000000000")]
        [InlineData("99999999999")]
        public void Criar_CpfInvalido_DeveLancarExcecao(string cpfInvalido)
        {
            // Arrange
            var cpfInvalidoValor = cpfInvalido;

            // Act
            var ex = Assert.Throws<InvalidValueException>(() => ClientePessoaFisica.Criar(
                TestData.NomeCompleto,
                cpfInvalidoValor,
                TestData.EmailValido,
                TestData.TelefoneValido,
                TestData.DataNascimento,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Criar_NomeVazio_DeveLancarExcecao()
        {
            // Arrange
            var nomeInvalido = string.Empty;

            // Act
            var ex = Assert.Throws<RequiredFieldException>(() => ClientePessoaFisica.Criar(
                nomeInvalido,
                TestData.CpfValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                TestData.DataNascimento,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }

        [Theory]
        [InlineData("email@")]
        [InlineData("login@domain..com")]
        [InlineData("login@domain.c")]
        public void Criar_EmailInvalido_DeveLancarExcecao(string emailInvalido)
        {
            // Arrange
            var emailInvalidoValor = emailInvalido;

            // Act
            var ex = Assert.Throws<InvalidValueException>(() => ClientePessoaFisica.Criar(
                TestData.NomeCompleto,
                TestData.CpfValido,
                emailInvalidoValor,
                TestData.TelefoneValido,
                TestData.DataNascimento,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }

        [Theory]
        [InlineData("1199-999")]
        [InlineData("123")]
        [InlineData("telefone")]
        public void Criar_TelefoneInvalido_DeveLancarExcecao(string telefoneInvalido)
        {
            // Arrange
            var telefoneInvalidoValor = telefoneInvalido;

            // Act
            var ex = Assert.Throws<InvalidValueException>(() => ClientePessoaFisica.Criar(
                TestData.NomeCompleto,
                TestData.CpfValido,
                TestData.EmailValido,
                telefoneInvalidoValor,
                TestData.DataNascimento,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Criar_DataNascimentoInvalida_DeveLancarExcecao()
        {
            // Arrange
            var dataNascimento = default(DateOnly);

            // Act
            var ex = Assert.Throws<RequiredFieldException>(() => ClientePessoaFisica.Criar(
                TestData.NomeCompleto,
                TestData.CpfValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                dataNascimento,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }
        #endregion

        #region Test Methods - Atualizar Valid Cases
        [Fact]
        public void Atualizar_DadosValidos_DeveAtualizarCliente()
        {
            // Arrange
            var cliente = ClientePessoaFisica.Criar(
                TestData.NomeCompleto,
                TestData.CpfValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                TestData.DataNascimento,
                _enderecoValido);

            var enderecoAtualizado = new Endereco(
                "Rua B",
                "200",
                "Apto 10",
                "02002000",
                "Rio de Janeiro",
                "RJ");

            // Act
            cliente.Atualizar(
                "Joao Atualizado",
                "joao.atualizado@email.com",
                "11988887777",
                new DateOnly(1991, 2, 2),
                enderecoAtualizado);

            // Assert
            Assert.Equal("Joao Atualizado", cliente.Nome);
            Assert.Equal("joao.atualizado@email.com", cliente.Email.Endereco);
            Assert.Equal("11988887777", cliente.Telefone.Numero);
            Assert.Equal(new DateOnly(1991, 2, 2), cliente.DataNascimento);
            Assert.Equal("Rua B", cliente.Endereco.Logradouro);
            Assert.NotNull(cliente.DataAtualizacao);
        }
        #endregion

        #region Test Methods - Atualizar Invalid Cases
        [Fact]
        public void Atualizar_EmailInvalido_DeveLancarExcecao()
        {
            // Arrange
            var cliente = ClientePessoaFisica.Criar(
                TestData.NomeCompleto,
                TestData.CpfValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                TestData.DataNascimento,
                _enderecoValido);

            // Act
            var ex = Assert.Throws<InvalidValueException>(() => cliente.Atualizar(
                TestData.NomeCompleto,
                "email@",
                TestData.TelefoneValido,
                TestData.DataNascimento,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }

        [Fact]
        public void Atualizar_DataNascimentoInvalida_DeveLancarExcecao()
        {
            // Arrange
            var cliente = ClientePessoaFisica.Criar(
                TestData.NomeCompleto,
                TestData.CpfValido,
                TestData.EmailValido,
                TestData.TelefoneValido,
                TestData.DataNascimento,
                _enderecoValido);

            // Act
            var ex = Assert.Throws<RequiredFieldException>(() => cliente.Atualizar(
                TestData.NomeCompleto,
                TestData.EmailValido,
                TestData.TelefoneValido,
                default,
                _enderecoValido));

            // Assert
            Assert.NotNull(ex);
        }
        #endregion
    }
}