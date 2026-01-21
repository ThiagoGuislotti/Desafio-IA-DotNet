using CustomerPlatform.Application.DependencyInjections;
using CustomerPlatform.Infrastructure.DependencyInjections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace CustomerPlatform.IntegrationTests.Assets
{
    /// <summary>
    /// Configura servicos para os testes de integracao.
    /// </summary>
    public class ConfigureServices
    {
        #region Public Properties
        /// <summary>
        /// Provedor de servicos configurado.
        /// </summary>
        public ServiceProvider ServiceProvider { get; protected set; }

        /// <summary>
        /// Configuracao carregada.
        /// </summary>
        public IConfigurationRoot Configuration { get; protected set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        public ConfigureServices()
        {
            var basePath = Directory.GetCurrentDirectory();
            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
                basePath = AppContext.BaseDirectory;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();
            var logLevel = LogLevel.Information;

            services.AddScoped(_ => LoggerFactory.Create(loggingBuilder =>
            {
                loggingBuilder
                    .ClearProviders()
                    .SetMinimumLevel(logLevel)
                    .AddConsole();
            }));

            services.AddCustomerPlatformApplication()
                .AddCustomerPlatformInfrastructure(Configuration);

            ServiceProvider = services.BuildServiceProvider();
        }
        #endregion
    }
}
