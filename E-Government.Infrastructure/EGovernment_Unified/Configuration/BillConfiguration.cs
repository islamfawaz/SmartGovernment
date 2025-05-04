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
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.ToTable("Bills");

            builder.HasKey(b => b.Id);

            // Properties configuration
            builder.Property(b => b.BillNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.IssueDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(b => b.DueDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(b => b.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // Relationships
            builder.HasOne(b => b.Meter)
                .WithMany(m => m.Bills)
                .HasForeignKey(b => b.MeterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bills)
                .HasForeignKey(b => b.UseNID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Payment)
                .WithOne(p => p.Bill)
                .HasForeignKey<Payment>(p => p.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(b => b.BillNumber).IsUnique();
            builder.HasIndex(b => b.Status);
            builder.HasIndex(b => b.DueDate);
            builder.HasIndex(b => b.UseNID);
            builder.HasIndex(b => b.MeterId);
        }
    }
}
