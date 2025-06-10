using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Government.Domain.Entities.CivilDocs;

namespace E_Government.Infrastructure.EGovernment_Unified.Configurations
{
    public class CivilDocumentAttachmentConfiguration : IEntityTypeConfiguration<CivilDocumentAttachment>
    {
        public void Configure(EntityTypeBuilder<CivilDocumentAttachment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.FileType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.UploadedAt)
                .IsRequired();

            builder.HasOne(a => a.Request)
                   .WithMany(r => r.Attachments)
                   .HasForeignKey(a => a.RequestId);
        }
    }
} 