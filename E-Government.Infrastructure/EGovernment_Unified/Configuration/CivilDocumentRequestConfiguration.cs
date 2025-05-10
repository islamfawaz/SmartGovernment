using E_Government.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EGovernment.Infrastructure.Configuration
{
    public class CivilDocumentRequestConfiguration : IEntityTypeConfiguration<CivilDocumentRequest>
    {
        public void Configure(EntityTypeBuilder<CivilDocumentRequest> builder)
        {
            builder.Property(r => r.ApplicantNID)
                .IsRequired()
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnType("nchar(50)");

            builder.Property(r => r.OwnerNID)
                .IsRequired()
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnType("nchar(50)");
        }
    }
} 