using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Government.Domain.Entities;

namespace E_Government.Infrastructure.Persistence._Data.Configuration
{
    
        public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
        {
            public void Configure(EntityTypeBuilder<ApplicationUser> builder)
            {
                builder.ToTable("AspNetUsers");


            builder.HasKey(u => u.NID);

            builder.Property(u => u.NID)
                .IsRequired()
                .HasMaxLength(50)
                .IsFixedLength()
                .ValueGeneratedNever();

            builder.Property(u => u.Address)
                    .IsRequired()
                    .HasMaxLength(200);

                //builder.Property(u => u.AccountNumber)
                //    .IsRequired()
                //    .HasMaxLength(20);

                builder.Property(u => u.Status)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

            builder.Property(u => u.CustomerCategory)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);


            //builder.Property(u => u.Status)
            //    .IsRequired()
            //    .HasConversion<string>()
            //    .HasMaxLength(20);

            // Relationships

            builder.HasMany(u => u.Meters)
                    .WithOne(m => m.User)
                    .HasForeignKey(m => m.UserNID)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(u => u.Bills)
                    .WithOne(b => b.User)
                    .HasForeignKey(b => b.UseNID)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r=>r.Requests)
                    .WithOne()
                    .HasForeignKey(r => r.ApplicantNID)
                    .HasPrincipalKey(u => u.NID)
                    .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(u => u.NID)
                    .IsUnique().IsUnique(); 

                //builder.HasIndex(u => u.AccountNumber)
                //    .IsUnique();

                //builder.HasIndex(u => u.Status);
            }
        }
}

