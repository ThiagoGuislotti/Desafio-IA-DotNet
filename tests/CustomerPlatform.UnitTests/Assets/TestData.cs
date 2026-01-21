namespace CustomerPlatform.UnitTests.Assets
{
    /// <summary>
    /// Dados comuns para testes.
    /// </summary>
    public static class TestData
    {
        #region Constants
        public const string NomeCompleto = "Joao Silva";
        public const string RazaoSocial = "Empresa Exemplo Ltda";
        public const string NomeFantasia = "Empresa Exemplo";
        public const string CpfValido = "12345678909";
        public const string CnpjValido = "86655518000189";
        public const string EmailValido = "joao@email.com";
        public const string TelefoneValido = "11999999999";
        public const string Logradouro = "Rua A";
        public const string NumeroEndereco = "100";
        public const string Cep = "01001000";
        public const string Cidade = "Sao Paulo";
        public const string Estado = "SP";
        #endregion

        #region Static Variables
        public static readonly DateOnly DataNascimento = new DateOnly(1990, 1, 1);
        #endregion
    }
}