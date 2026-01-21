using CustomerPlatform.Infrastructure.Data.Context.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerPlatform.Infrastructure.Data.Context.Mappings
{
    /// <summary>
    /// Mapeamento das suspeitas de duplicidade.
    /// </summary>
    public sealed class DuplicateSuspicionMapping : IEntityTypeConfiguration<DuplicateSuspicion>
    {
        #region Public Methods/Operators
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<DuplicateSuspicion> builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.ToTable("DuplicateSuspicions");

            builder.HasKey(suspicion => suspicion.Id);
            builder.Property(suspicion => suspicion.Id)
                .HasColumnName("duplicateSuspicionId")
                .ValueGeneratedNever();

            builder.Property(suspicion => suspicion.CustomerId)
                .HasColumnName("customerId")
                .IsRequired();

            builder.Property(suspicion => suspicion.CandidateCustomerId)
                .HasColumnName("candidateCustomerId")
                .IsRequired();

            builder.Property(suspicion => suspicion.Score)
                .HasColumnName("score")
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(suspicion => suspicion.Reason)
                .HasColumnName("reason")
                .HasMaxLength(500);

            builder.Property(suspicion => suspicion.CreatedAt)
                .HasColumnName("createdAt")
                .IsRequired();

            builder.HasIndex(suspicion => suspicion.CustomerId);
            builder.HasIndex(suspicion => suspicion.CandidateCustomerId);
        }
        #endregion
    }
}