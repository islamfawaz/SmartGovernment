using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Government.Domain.Entities.Bills;

namespace E_Government.Infrastructure.Persistence._Data.Configuration
{
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.ToTable("Bills");

            builder.HasKey(b => b.Id);

            // === Properties ===

            builder.Property(b => b.BillNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.ServiceCode)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.Description)
                .HasMaxLength(255);

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(b => b.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.PreviousReading)
                .HasPrecision(18, 2);

            builder.Property(b => b.CurrentReading)
                .HasPrecision(18, 2);

            builder.Property(b => b.IssueDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(b => b.DueDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(b => b.PaymentDate)
                .HasColumnType("datetime");

            builder.Property(b => b.StripePaymentId)
                .HasMaxLength(100);

            builder.Property(b => b.PaymentId)
                .HasMaxLength(100);

            builder.Property(b => b.PdfUrl)
                .HasMaxLength(255);

            // === Relationships ===

            builder.HasOne(b => b.Meter)
                .WithMany(m => m.Bills)
                .HasForeignKey(b => b.MeterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bills)
                .HasForeignKey(b => b.UseNID)
                .HasPrincipalKey(u => u.NID)
                .OnDelete(DeleteBehavior.Restrict);

            // === Indexes ===
            builder.HasIndex(b => b.BillNumber).IsUnique();
            builder.HasIndex(b => b.Status);
            builder.HasIndex(b => b.DueDate);
            builder.HasIndex(b => b.UseNID);
            builder.HasIndex(b => b.MeterId);
            builder.HasIndex(b => b.ServiceCode);
        }
    }
}
