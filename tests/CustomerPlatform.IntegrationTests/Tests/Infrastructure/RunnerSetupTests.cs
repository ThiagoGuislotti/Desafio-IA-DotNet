using CustomerPlatform.IntegrationTests.Assets;
using NUnit.Framework;

namespace CustomerPlatform.IntegrationTests.Tests.Infrastructure
{
    [TestFixture]
    [RequiresThread]
    [SetCulture("pt-BR")]
    [Category("Integration")]
    public sealed class RunnerSetupTests
    {
        #region Variables
        private string _environment = string.Empty;
        #endregion

        #region SetUp Methods
        [SetUp]
        public void SetUp()
        {
            _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        }
        #endregion

        #region Test Methods - RunnerSetup Valid Cases
        [Test]
        public void RunnerSetup_Environment_DeveEstarDefinido()
        {
            // Arrange
            var esperado = _environment;

            // Act
            var atual = _environment;

            // Assert
            Assert.That(atual, Is.Not.Empty);
            Assert.That(atual, Is.EqualTo(esperado));
            TestLogger.WriteLine($"Environment: {atual}");
        }
        #endregion
    }
}