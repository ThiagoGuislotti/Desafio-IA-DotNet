using System.Collections.Generic;
using CustomerPlatform.Domain.Exceptions;
using CustomerPlatform.Domain.ValueObjects;
using CustomerPlatform.UnitTests.Assets;
using Xunit;
using Xunit.Abstractions;

namespace CustomerPlatform.UnitTests.Tests.Domain.ValueObjects
{
    [Trait("Category", "Unit")]
    public sealed class EmailTests
    {
        #region Variables
        private readonly ITestOutputHelper _output;
        #endregion

        #region SetUp Methods
        public EmailTests(ITestOutputHelper output)
        {
            _output = output;
        }
        #endregion

        #region Test Methods - Constructor Valid Cases
        [Theory]
        [MemberData(nameof(ValidEmails))]
        public void Constructor_EmailValido_DeveCriarEmail(string email)
        {
            // Arrange
            var emailValido = email;

            // Act
            var emailValue = new Email(emailValido);

            // Assert
            Assert.Equal(emailValido, emailValue.Endereco);
            _output.WriteLine(emailValue.ToString());
        }
        #endregion

        #region Test Methods - Constructor Invalid Cases
        [Theory]
        [MemberData(nameof(InvalidEmails))]
        public void Constructor_EmailInvalido_DeveLancarExcecao(string email)
        {
            // Arrange
            var emailInvalido = email;

            // Act
            var ex = Assert.Throws<InvalidValueException>(() => new Email(emailInvalido));

            // Assert
            Assert.NotNull(ex);
        }
        #endregion

        #region Test Methods - Equals Valid Cases
        [Fact]
        public void Equals_EmailsIguais_DeveSerVerdadeiro()
        {
            // Arrange
            var emailA = new Email(TestData.EmailValido);
            var emailB = new Email(TestData.EmailValido);

            // Act
            var resultado = emailA == emailB;

            // Assert
            Assert.True(resultado);
            Assert.True(emailA.Equals(emailB));
            Assert.Equal(emailA.GetHashCode(), emailB.GetHashCode());
        }
        #endregion

        #region Test Cases
        public static IEnumerable<object[]> InvalidEmails
        {
            get
            {
                yield return new object[] { "invalid.email" };
                yield return new object[] { "user@missingtld." };
                yield return new object[] { ".@123.45.67.89" };
                yield return new object[] { "user@.com" };
                yield return new object[] { "user@com" };
                yield return new object[] { ".login@domain.com" };
                yield return new object[] { "-login@domain.com" };
                yield return new object[] { "_login@domain.com" };
                yield return new object[] { "login.@domain.com" };
                yield return new object[] { "login-@domain.com" };
                yield return new object[] { "login_@domain.com" };
                yield return new object[] { "login..name@domain.com" };
                yield return new object[] { "login--name@domain.com" };
                yield return new object[] { "login__name@domain.com" };
                yield return new object[] { "login_-name@domain.com" };
                yield return new object[] { "login_.name@domain.com" };
                yield return new object[] { "login@-domain.com" };
                yield return new object[] { "login@.domain.com" };
                yield return new object[] { "login@domain.com." };
                yield return new object[] { "login@dom_ain.com" };
                yield return new object[] { "login@domain..com" };
                yield return new object[] { "login@domain--domain2.com" };
                yield return new object[] { "login@domain.c" };
                yield return new object[] { "j***********a@g****.**m" };
                yield return new object[] { "" };
            }
        }

        public static IEnumerable<object[]> ValidEmails
        {
            get
            {
                yield return new object[] { "user@example.com" };
                yield return new object[] { "user123@example.co.uk" };
                yield return new object[] { "john.doe123@my-domain.org" };
                yield return new object[] { "user@192.168.0.1.io" };
                yield return new object[] { "login@domain.com" };
                yield return new object[] { "123login456@123domain456.com" };
                yield return new object[] { "login@dom1.dom2.dom-3.dom-4.com" };
                yield return new object[] { TestData.EmailValido };
            }
        }
        #endregion
    }
}
