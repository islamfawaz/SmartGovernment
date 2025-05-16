using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Government.Core.Domain.Entities.CivilDocs;

namespace E_Government.Infrastructure.EGovernment_Unified.Configurations
{
    public class CivilDocumentRequestConfiguration : IEntityTypeConfiguration<CivilDocumentRequest>
    {
        public void Configure(EntityTypeBuilder<CivilDocumentRequest> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.ApplicantName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.ApplicantNID)
                .IsRequired()
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnType("nchar(50)");

            builder.Property(r => r.Relation)
                .HasMaxLength(50);

            builder.Property(r => r.OwnerName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.OwnerNID)
                .IsRequired()
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnType("nchar(50)");

            builder.Property(r => r.OwnerMotherName)
                .HasMaxLength(100);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(r => r.CopiesCount)
                .IsRequired();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.Property(r => r.ExtraFieldsJson)
                .HasColumnName("ExtraFields")
                .HasColumnType("nvarchar(max)")
                .IsRequired(false)
                .HasDefaultValue("{}");

            builder.HasMany(r => r.Attachments)
                   .WithOne(a => a.Request)
                   .HasForeignKey(a => a.RequestId);

            builder.HasMany(r => r.History)
                   .WithOne(h => h.Request)
                   .HasForeignKey(h => h.RequestId);
        }
    }
} 