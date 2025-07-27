using E_Government.Domain.Entities;
using E_Government.Domain.Entities.Liscenses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.Persistence._Data.Configuration
{

    public class LicenseRequestHistoryConfiguration : IEntityTypeConfiguration<LicenseRequestHistory>
    {
        public void Configure(EntityTypeBuilder<LicenseRequestHistory> builder)
        {
            builder.ToTable("LicenseRequestHistories");
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .IsRequired()
                .ValueGeneratedNever();
         
            builder.Property(h => h.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(h => h.Note)
                .IsRequired()
                .HasMaxLength(500);

        
            builder.Property(h => h.ChangedAt)
                .IsRequired();

        
            // Foreign key relationships with license entities
            builder.HasOne(h => h.Request)
                .WithMany(l=>l.LicenseRequestHistories)
                .HasForeignKey(l=>l.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}