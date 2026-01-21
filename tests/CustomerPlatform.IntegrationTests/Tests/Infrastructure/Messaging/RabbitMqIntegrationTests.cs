using CustomerPlatform.Domain.Enums;
using CustomerPlatform.Domain.Events;
using CustomerPlatform.Infrastructure.Data.Context;
using CustomerPlatform.Infrastructure.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace CustomerPlatform.IntegrationTests.Tests.Infrastructure.Messaging
{
    [TestFixture]
    [RequiresThread]
    [SetCulture("pt-BR")]
    [Category("Infrastructure")]
    public sealed class RabbitMqIntegrationTests
    {
        #region Variables
        private CustomerPlatformDbContext _dbContext = null!;
        private RabbitMqOptions _rabbitOptions = null!;
        #endregion

        #region SetUp Methods
        [SetUp]
        public async Task SetUp()
        {
            var options = new DbContextOptionsBuilder<CustomerPlatformDbContext>()
                .UseNpgsql(GlobalSetup.PostgresConnectionString)
                .Options;

            _dbContext = new CustomerPlatformDbContext(options);
            await _dbContext.Database.MigrateAsync().ConfigureAwait(false);

            _rabbitOptions = new RabbitMqOptions
            {
                ConnectionString = GlobalSetup.RabbitMqConnectionString,
                ExchangeName = RabbitMqOptions.DefaultExchangeName,
                ExchangeType = "topic"
            };
        }

        [TearDown]
        public async Task TearDown()
        {
            var outbox = await _dbContext.OutboxEvents.ToListAsync().ConfigureAwait(false);
            if (outbox.Count > 0)
            {
                _dbContext.OutboxEvents.RemoveRange(outbox);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
        #endregion

        #region Test Methods - PublishEvent Valid Cases
        [Test]
        public async Task PublishEvent_ShouldBeConsumed()
        {
            // Arranjo
            using var publisher = new RabbitMqEventPublisher(Options.Create(_rabbitOptions));
            using var consumer = new RabbitMqEventConsumer(Options.Create(_rabbitOptions));
            var queueName = $"customerplatform-tests-{Guid.NewGuid():N}";
            var eventId = Guid.NewGuid();
            var domainEvent = new ClienteCriado(
                Guid.NewGuid(),
                TipoCliente.PF,
                "52998224725",
                "Cliente Teste",
                eventId);

            var tcs = new TaskCompletionSource<EventMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await consumer.StartAsync(queueName, (message, ct) =>
            {
                tcs.TrySetResult(message);
                return Task.CompletedTask;
            }, cts.Token).ConfigureAwait(false);

            // Acao
            await publisher.PublishAsync(domainEvent, cts.Token).ConfigureAwait(false);
            var completed = await Task.WhenAny(tcs.Task, Task.Delay(TimeSpan.FromSeconds(10), cts.Token)).ConfigureAwait(false);

            // Assertiva
            Assert.That(completed, Is.EqualTo(tcs.Task));
            var received = await tcs.Task.ConfigureAwait(false);
            Assert.That(received.EventId, Is.EqualTo(eventId));
        }
        #endregion

        #region Test Methods - OutboxWriter Valid Cases
        [Test]
        public async Task OutboxWriter_ShouldPersistEvent()
        {
            // Arranjo
            var eventId = Guid.NewGuid();
            var domainEvent = new ClienteCriado(
                Guid.NewGuid(),
                TipoCliente.PF,
                "52998224725",
                "Cliente Teste",
                eventId);

            var writer = new OutboxWriter(_dbContext);

            // Acao
            await writer.EnqueueAsync(domainEvent, CancellationToken.None).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            var stored = await _dbContext.OutboxEvents
                .AsNoTracking()
                .FirstOrDefaultAsync(current => current.EventId == eventId)
                .ConfigureAwait(false);

            // Assertiva
            Assert.That(stored, Is.Not.Null);
            Assert.That(stored!.ProcessedAt, Is.Null);
        }
        #endregion
    }
}
