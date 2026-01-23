using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Sinks.OpenTelemetry;
using System.Diagnostics;

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

            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;

            var otlpEndpoint = configuration.GetConnectionString("Otlp");
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithIgnoreStackTraceAndTargetSiteExceptionFilter()
                    .WithDefaultDestructurers())
                .Enrich.WithDemystifiedStackTraces()
                .Enrich.WithProcessId()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithClientIp()
                .Enrich.WithProperty("service.name", serviceName)
                .Enrich.WithProperty("deployment.environment", environmentName)
                .Enrich.WithProperty("service.instance.id", Environment.MachineName)
                .WriteTo.Async(asyncOptions => asyncOptions.OpenTelemetry(options =>
                {
                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                        options.Endpoint = otlpEndpoint;

                    options.Protocol = OtlpProtocol.Grpc;
                    options.IncludedData = IncludedData.MessageTemplateTextAttribute
                        | IncludedData.MessageTemplateMD5HashAttribute
                        | IncludedData.MessageTemplateRenderingsAttribute
                        | IncludedData.SpanIdField
                        | IncludedData.TraceIdField
                        | IncludedData.SpecRequiredResourceAttributes;
                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = serviceName,
                        ["deployment.environment"] = environmentName,
                        ["service.instance.id"] = Environment.MachineName
                    };
                }))
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(Log.Logger, dispose: true);
            });

            var otelBuilder = services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName).AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = environmentName,
                    ["service.instance.id"] = Environment.MachineName
                }));

            otelBuilder.WithTracing(tracing =>
            {
                tracing
                    .SetSampler(new AlwaysOnSampler())
                    .SetErrorStatusOnException()
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = context =>
                        {
                            var path = context.Request.Path.Value ?? string.Empty;
                            return !(path.EndsWith(".css", StringComparison.OrdinalIgnoreCase)
                                || path.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
                                || path.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)
                                || path.EndsWith(".json", StringComparison.OrdinalIgnoreCase));
                        };
                        options.EnrichWithHttpRequest = (activity, request) =>
                            activity.SetTag("request.protocol", request.Protocol);
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exception.stackTrace", exception.StackTrace);
                            activity.SetTag("exception.source", exception.Source);
                        };
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exception.stackTrace", exception.StackTrace);
                            activity.SetTag("exception.source", exception.Source);
                        };
                    })
                    .AddEntityFrameworkCoreInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

            otelBuilder.WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddProcessInstrumentation()
                    .AddRuntimeInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    metrics.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

            return services;
        }
        #endregion
    }
}