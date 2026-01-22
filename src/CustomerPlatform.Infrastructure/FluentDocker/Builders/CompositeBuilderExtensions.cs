using Ductus.FluentDocker.Builders;
using System.Text.RegularExpressions;

namespace CustomerPlatform.Infrastructure.FluentDocker.Builders
{
    /// <summary>
    /// Extensoes do CompositeBuilder para carregar variaveis de ambiente do .env.
    /// </summary>
    public static class CompositeBuilderExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Adiciona variaveis de ambiente de um arquivo .env, resolvendo referencias ${VAR}.
        /// </summary>
        /// <param name="builder">Builder do docker compose.</param>
        /// <param name="envFilePath">Caminho do arquivo .env.</param>
        /// <returns>Builder atualizado.</returns>
        /// <exception cref="FileNotFoundException">Lancado quando o arquivo nao existe.</exception>
        /// <exception cref="ArgumentException">Lancado quando o caminho e invalido.</exception>
        public static CompositeBuilder WithResolvedEnvironment(
            this CompositeBuilder builder,
            string envFilePath)
        {
            if (string.IsNullOrWhiteSpace(envFilePath))
                return builder;

            var envVariables = File.ReadAllLines(envFilePath)
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith('#'))
                .Select(line => line.Split('=', 2))
                .Where(parts => parts.Length == 2)
                .Select(parts => new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim()))
                .Where(kv => !string.Equals(kv.Key, "COMPOSE_PROJECT_NAME", StringComparison.OrdinalIgnoreCase));

            string resolveVariables(string value)
            {
                var pattern = @"\${([^}]+)}";
                var matches = Regex.Matches(value, pattern);

                foreach (Match match in matches)
                {
                    var varName = match.Groups[1].Value;
                    var resolvedValue = envVariables.FirstOrDefault(x => x.Key == varName).Value;

                    if (resolvedValue != null)
                        value = value.Replace(match.Value, resolvedValue);
                }

                return value;
            }

            var resolvedEnvVariables = envVariables
                .Select(kv => $"{kv.Key}={resolveVariables(kv.Value)}")
                .ToArray();

            return resolvedEnvVariables.Length > 0
                ? builder.WithEnvironment(resolvedEnvVariables)
                : builder;
        }

        /// <summary>
        /// Aplica substituicoes em um docker-compose e usa o arquivo temporario resultante.
        /// </summary>
        /// <param name="builder">Builder do docker compose.</param>
        /// <param name="originalFilePath">Caminho do arquivo original.</param>
        /// <param name="replacements">Substituicoes a aplicar.</param>
        /// <returns>Builder atualizado.</returns>
        /// <exception cref="FileNotFoundException">Lancado quando o arquivo nao existe.</exception>
        /// <exception cref="ArgumentException">Lancado quando os parametros sao invalidos.</exception>
        public static CompositeBuilder WithResolvedFile(
            this CompositeBuilder builder,
            string originalFilePath,
            params (string OldValue, string NewValue)[] replacements)
        {
            if (string.IsNullOrWhiteSpace(originalFilePath))
                throw new ArgumentException("Original file path cannot be null or empty.", nameof(originalFilePath));

            if (!File.Exists(originalFilePath))
                throw new FileNotFoundException($"Docker Compose file not found: {originalFilePath}", originalFilePath);

            if (replacements == null || replacements.Length == 0)
                throw new ArgumentException("At least one replacement must be provided.", nameof(replacements));

            var content = File.ReadAllText(originalFilePath);

            foreach (var (OldValue, NewValue) in replacements)
            {
                if (string.IsNullOrEmpty(OldValue))
                    throw new ArgumentException("Old value cannot be null or empty in replacement.");

                content = content.Replace(OldValue, NewValue);
            }

            var outputDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var originalFileName = Path.GetFileNameWithoutExtension(originalFilePath);
            var extension = Path.GetExtension(originalFilePath);
            var timestamp = DateTime.Now.Ticks;
            var tempFileName = $"{originalFileName}-{timestamp}{extension}";
            var tempFilePath = Path.Combine(outputDirectory, tempFileName);

            File.WriteAllText(tempFilePath, content);

            return builder.FromFile(tempFilePath);
        }
        #endregion
    }
}