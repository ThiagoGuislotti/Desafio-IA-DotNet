using CustomerPlatform.Api;
using CustomerPlatform.Api.Middlewares;
using CustomerPlatform.Api.Swagger;
using CustomerPlatform.Application.DependencyInjections;
using CustomerPlatform.Infrastructure.DependencyInjections;
using CustomerPlatform.Infrastructure.Observability;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCustomerPlatformObservability(builder.Configuration, "CustomerPlatform.Api");
builder.Services.AddCustomerPlatformApplication();
builder.Services.AddCustomerPlatformInfrastructure(builder.Configuration);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddCustomHostedService();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<SwaggerExamplesSchemaFilter>();
    options.OperationFilter<SwaggerExamplesOperationFilter>();
});
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("PostgreSql") ?? string.Empty,
        name: "postgresql")
    .AddRabbitMQ(
        builder.Configuration.GetConnectionString("RabbitMq") ?? string.Empty,
        name: "rabbitmq")
    .AddElasticsearch(
        builder.Configuration.GetConnectionString("ElasticSearch") ?? string.Empty,
        name: "elasticsearch");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => app.Logger.LogInformation("[Inicio:Api]-[{Environment}] - [{Application}]", app.Environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName));
lifetime.ApplicationStopping.Register(() => app.Logger.LogInformation("[Parando:Api]-[{Environment}] - [{Application}]", app.Environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName));
lifetime.ApplicationStopped.Register(() => app.Logger.LogInformation("[Parado:Api]-[{Environment}] - [{Application}]", app.Environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName));

await app.RunAsync();

public partial class Program
{
}