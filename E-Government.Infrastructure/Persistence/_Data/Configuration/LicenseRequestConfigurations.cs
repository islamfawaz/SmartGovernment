using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.Liscenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Government.Infrastructure.Persistence._Data.Configuration
{
    public class LicenseRequestConfigurations : IEntityTypeConfiguration<LicenseRequest>
    {
        public void Configure(EntityTypeBuilder<LicenseRequest> builder)
        {
            builder.ToTable("LicenseRequests");
            builder.HasKey(lr => lr.Id);
            builder.Property(r => r.LicenseType)
              .IsRequired()
              .HasMaxLength(50);

            builder.Property(lr=>lr.ServiceCode)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(l=>l.Status)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(l=>l.Notes)
                .HasMaxLength(500);

            builder.Property(l => l.UploadedDocumentUrl)
                .HasMaxLength(500);

            builder.Property(r => r.ExtraFieldsJson)
                         .HasColumnName("ExtraFields")
                         .HasColumnType("nvarchar(max)")
                         .HasDefaultValue("{}")
                         .IsRequired(false);

            builder.HasMany(l=>l.LicenseRequestHistories)
                .WithOne(h=>h.Request)
                .HasForeignKey(l=>l.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(l => l.Bill)
               .WithOne(b => b.Request)
               .HasForeignKey<Bill>(b => b.RequestId);
        }
    }
}
