using CustomerPlatform.Infrastructure.Data.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerPlatform.Infrastructure.Data.Context.Mappings
{
    /// <summary>
    /// Mapeamento de eventos do outbox.
    /// </summary>
    public sealed class OutboxEventMapping : IEntityTypeConfiguration<OutboxEvent>
    {
        #region Public Methods/Operators
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<OutboxEvent> builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.ToTable("OutboxEvents");

            builder.HasKey(outbox => outbox.Id);
            builder.Property(outbox => outbox.Id)
                .HasColumnName("outboxEventId")
                .ValueGeneratedNever();

            builder.Property(outbox => outbox.EventId)
                .HasColumnName("eventId")
                .IsRequired();

            builder.Property(outbox => outbox.EventType)
                .HasColumnName("eventType")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(outbox => outbox.Payload)
                .HasColumnName("payload")
                .HasColumnType("jsonb")
                .IsRequired();

            builder.Property(outbox => outbox.OccurredAt)
                .HasColumnName("occurredAt")
                .IsRequired();

            builder.Property(outbox => outbox.CreatedAt)
                .HasColumnName("createdAt")
                .IsRequired();

            builder.Property(outbox => outbox.ProcessedAt)
                .HasColumnName("processedAt");

            builder.Property(outbox => outbox.RetryCount)
                .HasColumnName("retryCount")
                .IsRequired();

            builder.Property(outbox => outbox.LastError)
                .HasColumnName("lastError")
                .HasMaxLength(2000);

            builder.HasIndex(outbox => outbox.EventId)
                .IsUnique();
        }
        #endregion
    }
}