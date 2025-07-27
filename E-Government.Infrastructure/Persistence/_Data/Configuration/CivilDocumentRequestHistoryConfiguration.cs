using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Government.Domain.Entities.CivilDocs;

namespace E_Government.Infrastructure.Persistence._Data.Configuration
{
    public class CivilDocumentRequestHistoryConfiguration : IEntityTypeConfiguration<CivilDocumentRequestHistory>
    {
        public void Configure(EntityTypeBuilder<CivilDocumentRequestHistory> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Status)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(h => h.Note)
                .HasMaxLength(500);

            builder.Property(h => h.ChangedAt)
                .IsRequired();

            builder.HasOne(h => h.Request)
                   .WithMany(r => r.History)
                   .HasForeignKey(h => h.RequestId);
        }
    }
} 