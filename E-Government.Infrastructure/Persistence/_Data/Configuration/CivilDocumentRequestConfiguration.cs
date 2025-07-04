using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Government.Domain.Entities.CivilDocs;

namespace E_Government.Infrastructure.Persistence._Data.Configuration
{
    public class CivilDocumentRequestConfiguration : IEntityTypeConfiguration<CivilDocumentRequest>
    {
        public void Configure(EntityTypeBuilder<CivilDocumentRequest> builder)
        {
            // Table and Key
            builder.ToTable("CivilDocumentRequests");
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.ApplicantName)
                .IsRequired()
                .HasMaxLength(100);

            // Configure the foreign key column to match AspNetUsers.NID
            builder.Property(r => r.ApplicantNID)
                .IsRequired()
                .HasColumnType("nvarchar(50)");

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
                .HasDefaultValue("{}")
                .IsRequired(false);

            // Relationships
            builder.HasMany(r => r.Attachments)
                .WithOne(a => a.Request)
                .HasForeignKey(a => a.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.History)
                .WithOne(h => h.Request)
                .HasForeignKey(h => h.RequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
