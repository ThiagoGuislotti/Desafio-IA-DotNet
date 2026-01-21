using CustomerPlatform.Infrastructure.DependencyInjections;
using CustomerPlatform.Infrastructure.Observability;
using CustomerPlatform.Worker.DependencyInjections;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCustomerPlatformObservability(builder.Configuration, "CustomerPlatform.Worker");
builder.Services.AddCustomerPlatformInfrastructure(builder.Configuration);
builder.Services.AddCustomerPlatformWorker(builder.Configuration);

var host = builder.Build();
await host.RunAsync().ConfigureAwait(false);
