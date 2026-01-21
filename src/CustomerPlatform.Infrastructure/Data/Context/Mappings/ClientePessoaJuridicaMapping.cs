using CustomerPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerPlatform.Infrastructure.Data.Context.Mappings
{
    /// <summary>
    /// Mapeamento de cliente pessoa juridica.
    /// </summary>
    public sealed class ClientePessoaJuridicaMapping : IEntityTypeConfiguration<ClientePessoaJuridica>
    {
        #region Public Methods/Operators
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ClientePessoaJuridica> builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Property(customer => customer.RazaoSocial)
                .HasColumnName("corporateName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(customer => customer.NomeFantasia)
                .HasColumnName("tradeName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(customer => customer.Documento)
                .HasColumnName("document")
                .HasMaxLength(14)
                .HasConversion(ValueObjectConverters.DocumentoConverter)
                .IsRequired();
        }
        #endregion
    }
}