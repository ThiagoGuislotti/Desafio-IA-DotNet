using CustomerPlatform.Api;
using CustomerPlatform.Api.Middlewares;
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
builder.Services.AddSwaggerGen();
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

app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();

public partial class Program
{
}
