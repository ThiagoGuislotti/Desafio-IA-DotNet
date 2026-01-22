using System.Diagnostics.CodeAnalysis;

namespace CustomerPlatform.IntegrationTests.Assets.FluentDocker.Utilities
{
    /// <summary>
    /// Utilitario para localizar diretorios dentro do repositorio.
    /// </summary>
    public static class DirectoryLocator
    {
        #region Constants
        /// <summary>
        /// Numero maximo de niveis para busca.
        /// </summary>
        private const int DefaultMaxLevelsToSearch = 10;
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Tenta localizar um diretorio subindo a partir do diretorio atual.
        /// </summary>
        /// <param name="pathSegments">Segmentos do caminho alvo.</param>
        /// <param name="foundPath">Caminho encontrado.</param>
        /// <param name="maxLevels">Numero maximo de niveis.</param>
        /// <returns>true quando encontrado; caso contrario false.</returns>
        /// <exception cref="ArgumentNullException">Lancado quando os segmentos sao nulos.</exception>
        /// <exception cref="ArgumentException">Lancado quando os segmentos estao vazios.</exception>
        public static bool TryLocateDirectory(
            string[] pathSegments,
            [NotNullWhen(true)] out string? foundPath,
            int maxLevels = DefaultMaxLevelsToSearch)
        {
            ArgumentNullException.ThrowIfNull(pathSegments);

            if (pathSegments.Length == 0 || pathSegments.All(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Path segments cannot be empty or contain only whitespace.", nameof(pathSegments));

            foundPath = null;
            var currentDir = Directory.GetCurrentDirectory();
            var targetPath = Path.Combine(pathSegments);

            for (var level = 0; level <= maxLevels; level++)
            {
                var searchPath = level == 0
                    ? Path.Combine(currentDir, targetPath)
                    : Path.GetFullPath(Path.Combine(currentDir, string.Concat(Enumerable.Repeat("../", level)), targetPath));

                if (Directory.Exists(searchPath))
                {
                    foundPath = searchPath;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Localiza um diretorio subindo a partir do diretorio atual.
        /// </summary>
        /// <param name="pathSegments">Segmentos do caminho alvo.</param>
        /// <param name="maxLevels">Numero maximo de niveis.</param>
        /// <returns>Caminho completo do diretorio encontrado.</returns>
        /// <exception cref="ArgumentNullException">Lancado quando os segmentos sao nulos.</exception>
        /// <exception cref="ArgumentException">Lancado quando os segmentos estao vazios.</exception>
        /// <exception cref="DirectoryNotFoundException">Lancado quando o diretorio nao foi encontrado.</exception>
        public static string LocateDirectory(
            string[] pathSegments,
            int maxLevels = DefaultMaxLevelsToSearch)
        {
            if (!TryLocateDirectory(pathSegments, out var foundPath, maxLevels))
            {
                var targetPath = Path.Combine(pathSegments);
                throw new DirectoryNotFoundException(
                    $"Could not locate directory '{targetPath}' within {maxLevels} parent directory levels from '{Directory.GetCurrentDirectory()}'.");
            }

            return foundPath;
        }
        #endregion
    }
}