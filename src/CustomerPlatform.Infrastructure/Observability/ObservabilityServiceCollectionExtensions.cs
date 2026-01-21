using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;

namespace CustomerPlatform.Infrastructure.Observability
{
    /// <summary>
    /// Extensoes de observabilidade.
    /// </summary>
    public static class ObservabilityServiceCollectionExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Configura Serilog e OpenTelemetry.
        /// </summary>
        /// <param name="services">Collection de servicos.</param>
        /// <param name="configuration">Configuracao da aplicacao.</param>
        /// <param name="serviceName">Nome do servico.</param>
        /// <returns>Collection atualizada.</returns>
        public static IServiceCollection AddCustomerPlatformObservability(
            this IServiceCollection services,
            IConfiguration configuration,
            string serviceName)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Nome do servico obrigatorio.", nameof(serviceName));

            var otlpEndpoint = configuration.GetConnectionString("Otlp");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("Application", serviceName)
                .WriteTo.Console()
                .WriteTo.OpenTelemetry(options =>
                {
                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                        options.Endpoint = otlpEndpoint;
                })
                .CreateLogger();

            services.AddLogging(builder => builder.AddSerilog(Log.Logger, dispose: true));

            var otelBuilder = services.AddOpenTelemetry();
            otelBuilder.WithTracing(tracing =>
            {
                tracing.AddHttpClientInstrumentation();
                tracing.AddEntityFrameworkCoreInstrumentation();
                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    tracing.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
            });

            otelBuilder.WithMetrics(metrics =>
            {
                metrics.AddHttpClientInstrumentation();
                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    metrics.AddOtlpExporter(options => options.Endpoint = new Uri(otlpEndpoint));
            });

            return services;
        }
        #endregion
    }
}