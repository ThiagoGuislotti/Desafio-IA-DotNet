using CustomerPlatform.Domain.ValueObjects;

namespace CustomerPlatform.UnitTests.Assets
{
    /// <summary>
    /// Factory de endereco para testes.
    /// </summary>
    public static class EnderecoFactory
    {
        #region Public Methods/Operators
        public static Endereco CriarValido()
        {
            return new Endereco(
                TestData.Logradouro,
                TestData.NumeroEndereco,
                null,
                TestData.Cep,
                TestData.Cidade,
                TestData.Estado);
        }
        #endregion
    }
}