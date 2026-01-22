using System.Linq;

namespace CustomerPlatform.IntegrationTests.Assets
{
    /// <summary>
    /// Utilitario para localizar diretorios a partir do diretorio atual.
    /// </summary>
    public static class DirectoryLocator
    {
        #region Constants
        /// <summary>
        /// Maximo de niveis a subir na busca.
        /// </summary>
        private const int DefaultMaxLevelsToSearch = 10;
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Localiza um diretorio navegando para cima a partir do diretorio atual.
        /// </summary>
        /// <param name="pathSegments">Segmentos do caminho do diretorio.</param>
        /// <param name="maxLevels">Quantidade maxima de niveis para buscar.</param>
        /// <returns>Caminho completo do diretorio encontrado.</returns>
        /// <exception cref="ArgumentNullException">Lancado quando pathSegments e nulo.</exception>
        /// <exception cref="ArgumentException">Lancado quando os segmentos sao invalidos.</exception>
        /// <exception cref="DirectoryNotFoundException">Lancado quando o diretorio nao e encontrado.</exception>
        public static string LocateDirectory(
            string[] pathSegments,
            int maxLevels = DefaultMaxLevelsToSearch)
        {
            ArgumentNullException.ThrowIfNull(pathSegments);

            if (pathSegments.Length == 0 || pathSegments.All(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Segmentos obrigatorios.", nameof(pathSegments));

            var currentDir = Directory.GetCurrentDirectory();
            var targetPath = Path.Combine(pathSegments);

            for (var level = 0; level <= maxLevels; level++)
            {
                var searchPath = level == 0
                    ? Path.Combine(currentDir, targetPath)
                    : Path.GetFullPath(Path.Combine(currentDir, string.Concat(Enumerable.Repeat("../", level)), targetPath));

                if (Directory.Exists(searchPath))
                    return searchPath;
            }

            throw new DirectoryNotFoundException(
                $"Diretorio '{targetPath}' nao encontrado a partir de '{currentDir}'.");
        }
        #endregion
    }
}