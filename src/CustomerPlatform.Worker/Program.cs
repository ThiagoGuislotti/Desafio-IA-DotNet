using CustomerPlatform.Infrastructure.DependencyInjections;
using CustomerPlatform.Infrastructure.Observability;
using CustomerPlatform.Worker.DependencyInjections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddConfiguration(configuration);
builder.Services.AddCustomerPlatformObservability(configuration, "CustomerPlatform.Worker");
builder.Services.AddCustomerPlatformInfrastructure(configuration);
builder.Services.AddCustomerPlatformWorker(configuration);

var host = builder.Build();

var environment = host.Services.GetRequiredService<IHostEnvironment>();
var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("CustomerPlatform.Worker");
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => logger.LogInformation("[Inicio:Worker]-[{Environment}] - [{Application}]", environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName));
lifetime.ApplicationStopping.Register(() => logger.LogInformation("[Parando:Worker]-[{Environment}] - [{Application}]", environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName));
lifetime.ApplicationStopped.Register(() => logger.LogInformation("[Parado:Worker]-[{Environment}] - [{Application}]", environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName));

await host.RunAsync().ConfigureAwait(false);