using CustomerPlatform.Application.Abstractions;
using CustomerPlatform.Application.Abstractions.Deduplication;
using CustomerPlatform.Application.Abstractions.Messaging;
using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.Deduplication;
using CustomerPlatform.Infrastructure.Messaging;
using CustomerPlatform.Infrastructure.Search;
using CustomerPlatform.Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CustomerPlatform.Infrastructure.DependencyInjections
{
    /// <summary>
    /// Extensoes de DI para a camada Infrastructure.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Registra servicos da camada Infrastructure.
        /// </summary>
        /// <param name="services">Collection de servicos.</param>
        /// <param name="configuration">Configuracao da aplicacao.</param>
        /// <returns>Collection atualizada.</returns>
        public static IServiceCollection AddCustomerPlatformInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            var connectionString = configuration.GetConnectionString("PostgreSql");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("ConnectionString PostgreSql obrigatoria.");

            services.AddDbContext<CustomerPlatformDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    npgsql => npgsql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(2), null)));

            services.Configure<RabbitMqOptions>(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("RabbitMq") ?? string.Empty;
                options.ExchangeName = configuration["RabbitMq:ExchangeName"] ?? RabbitMqOptions.DefaultExchangeName;
                options.QueueName = configuration["RabbitMq:QueueName"] ?? options.QueueName;
            });

            services.Configure<ElasticSearchOptions>(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("ElasticSearch") ?? string.Empty;
                options.IndexName = configuration["ElasticSearch:IndexName"] ?? options.IndexName;
                options.Username = configuration["ElasticSearch:Username"];
                options.Password = configuration["ElasticSearch:Password"];
            });

            services.Configure<DeduplicationOptions>(configuration.GetSection("Deduplication"));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEventPublisher, RabbitMqEventPublisher>();
            services.AddScoped<IOutboxWriter, OutboxWriter>();
            services.AddScoped<ICustomerSearchService, ElasticCustomerSearchService>();
            services.AddScoped<CustomerEventDispatcher>();
            services.AddScoped<DeduplicationProcessor>();
            services.AddScoped<IDeduplicationService, DeduplicationService>();
            services.AddSingleton<RabbitMqEventConsumer>();
            services.AddSingleton<ElasticSearchInitializer>();

            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ElasticSearchOptions>>();
                return ElasticClientFactory.Create(options.Value);
            });

            return services;
        }
        #endregion
    }
}
