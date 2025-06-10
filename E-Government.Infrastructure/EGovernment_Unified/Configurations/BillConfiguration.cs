using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Government.Domain.Entities.Bills;

namespace E_Government.Infrastructure.EGovernment_Unified.Configurations
{
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.CurrentReading)
                .HasPrecision(18, 2);

            builder.Property(b => b.PreviousReading)
                .HasPrecision(18, 2);

            builder.Property(b => b.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(b => b.Amount)
                .HasPrecision(18, 2);

            builder.Property(b => b.BillNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.Status)
                .IsRequired();

            builder.Property(b => b.IssueDate)
                .IsRequired();

            builder.Property(b => b.DueDate)
                .IsRequired();

            builder.HasOne(b => b.Meter)
                .WithMany(m => m.Bills)
                .HasForeignKey(b => b.MeterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bills)
                .HasForeignKey(b => b.UseNID)
                .HasPrincipalKey(u => u.NID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 