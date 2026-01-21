using NUnit.Framework;

namespace CustomerPlatform.IntegrationTests.Assets
{
    /// <summary>
    /// Logger simples para testes de integracao.
    /// </summary>
    public static class TestLogger
    {
        #region Public Methods/Operators
        public static void WriteLine(string message)
        {
            TestContext.WriteLine(message);
        }
        #endregion
    }
}