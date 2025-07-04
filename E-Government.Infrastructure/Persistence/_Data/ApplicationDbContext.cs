using E_Government.Domain.Entities;
using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.CivilDocs;
using E_Government.Domain.Entities.Liscenses;
using E_Government.Domain.Entities.OTP;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace E_Government.Infrastructure.Persistence._Data
{
   public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContext) :base(dbContext)
        {
          
        }

        public DbSet<Meter> Meters { get; set; }
        public DbSet<Bill> Bills { get; set; }

        public DbSet<ServicePrice> ServicePrices { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<MeterReading> MeterReadings { get; set; }

        public DbSet<CivilDocumentRequest> CivilDocumentRequests { get; set; }
        public DbSet<CivilDocumentAttachment> CivilDocumentAttachments { get; set; }
        public DbSet<CivilDocumentRequestHistory> CivilDocumentRequestHistories { get; set; }

        public DbSet<LicenseRequest>  LicenseRequests { get; set; }

        public DbSet<LicenseRequestHistory> LicenseRequestHistories { get; set; }

        public DbSet<OtpCode> OtpCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ServicePrice>().HasData(
      new ServicePrice
      {
          Id = 1,
          ServiceCode = "DRIVING_RENEW",
          ServiceName = "تجديد رخصة القيادة",
          Price = 100.00m,
          IsActive = true
      },
      new ServicePrice
      {
          Id = 2,
          ServiceCode = "DRIVING_REPLACE_LOST",
          ServiceName = "بدل فاقد لرخصة القيادة",
          Price = 120.00m,
          IsActive = true
      },
      new ServicePrice
      {
          Id = 3,
          ServiceCode = "DRIVING_NEW",
          ServiceName = "استخراج رخصة قيادة جديدة",
          Price = 200.00m,
          IsActive = true
      },
      new ServicePrice
      {
          Id = 4,
          ServiceCode = "LICENSE_DIGITAL",
          ServiceName = "رخصة رقمية",
          Price = 80.00m,
          IsActive = true
      },
      new ServicePrice
      {
          Id = 5,
          ServiceCode = "VEHICLE_RENEW",
          ServiceName = "تجديد رخصة مركبة",
          Price = 150.00m,
          IsActive = true
      },
      new ServicePrice
      {
          Id = 6,
          ServiceCode = "VEHICLE_NEW",
          ServiceName = "رخصة مركبة جديدة",
          Price = 180.00m,
          IsActive = true
      },
      new ServicePrice
      {
          Id = 7,
          ServiceCode = "TRAFFIC_FINE_VIEW",
          ServiceName = "الاستعلام عن مخالفات المرور",
          Price = 0.00m,
          IsActive = true
      },
      new ServicePrice
      {
          Id = 8,
          ServiceCode = "TRAFFIC_FINE_PAY",
          ServiceName = "دفع مخالفات المرور",
          Price = 50.00m,
          IsActive = true
      }
  );



            // Configure Identity tables to use NID as foreign key
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("AspNetUserRoles");
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                // Configure the foreign key to reference NID instead of Id
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .HasPrincipalKey(u => u.NID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("AspNetUserClaims");
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(uc => uc.UserId)
                    .HasPrincipalKey(u => u.NID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("AspNetUserLogins");
                entity.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(ul => ul.UserId)
                    .HasPrincipalKey(u => u.NID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("AspNetUserTokens");
                entity.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });
                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(ut => ut.UserId)
                    .HasPrincipalKey(u => u.NID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Apply other configurations
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

    }
}
