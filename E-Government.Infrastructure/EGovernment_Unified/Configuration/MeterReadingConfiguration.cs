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
    public class MeterReadingConfiguration : IEntityTypeConfiguration<MeterReading>
    {
        public void Configure(EntityTypeBuilder<MeterReading> builder)
        {
            builder.ToTable("MeterReadings");

            builder.HasKey(mr => mr.Id);

            builder.Property(mr => mr.ReadingDate)
                .IsRequired()
                .HasColumnType("datetime");

            builder.Property(mr => mr.Value)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Relationships
            builder.HasOne(mr => mr.Meter)
                .WithMany(m => m.Readings)
                .HasForeignKey(mr => mr.MeterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(mr => mr.ReadingDate);
        }
    }
}
