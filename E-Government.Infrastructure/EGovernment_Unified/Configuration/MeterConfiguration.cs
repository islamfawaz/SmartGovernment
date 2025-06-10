using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Government.Domain.Entities.Bills;

namespace E_Government.Infrastructure.EGovernment_Unified.Configuration
{
    public class MeterConfiguration : IEntityTypeConfiguration<Meter>
    {
        public void Configure(EntityTypeBuilder<Meter> builder)
        {
            builder.ToTable("Meters");

            builder.HasKey(m => m.Id);
            builder.Property(m => m.Id)
                .UseIdentityColumn();

            builder.Property(m => m.MeterNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            // Relationships
            builder.HasOne(m => m.User)
                .WithMany(u => u.Meters)
                .HasForeignKey(m => m.UserNID)
                .OnDelete(DeleteBehavior.Restrict);

           

            // Indexes
            builder.HasIndex(m => m.MeterNumber)
                .IsUnique();
        }
    }
}
