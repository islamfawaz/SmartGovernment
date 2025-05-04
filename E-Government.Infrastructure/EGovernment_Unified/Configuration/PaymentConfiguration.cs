using E_Government.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.EGovernment_Unified.Configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PaymentDate)
                .IsRequired();

            builder.Property(p => p.TransactionId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserNID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Bill)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.TransactionId)
                .IsUnique();
        }
    }
}
