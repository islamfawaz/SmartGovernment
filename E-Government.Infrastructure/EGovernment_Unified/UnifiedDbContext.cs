using E_Government.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Government.Infrastructure.EGovernment_Unified
{
   public class UnifiedDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {

        public UnifiedDbContext(DbContextOptions<UnifiedDbContext> dbContext) :base(dbContext)
        {
          
        }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<DrivingLicense> DrivingLicenses { get; set; }
        public DbSet<DrivingLicenseRenewal> DrivingLicenseRenewals { get; set; }
        public DbSet<VehicleLicenseRenewal> VehicleLicenseRenewals { get; set; }

        public DbSet<TrafficViolationPayment> TrafficViolationPayments { get; set; }
        public DbSet<LicenseReplacementRequest> LicenseReplacementRequests { get; set; }

        public DbSet<VehicleOwner> VehicleOwners { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<MeterReading> MeterReadings { get; set; }

        public DbSet<CivilDocumentRequest> CivilDocumentRequests { get; set; }
        public DbSet<CivilDocumentAttachment> CivilDocumentAttachments { get; set; }
        public DbSet<CivilDocumentRequestHistory> CivilDocumentRequestHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(UnifiedDbContext).Assembly);

            builder.Entity<CivilDocumentRequest>()
                .HasMany(r => r.Attachments)
                .WithOne(a => a.Request)
                .HasForeignKey(a => a.RequestId);

            builder.Entity<CivilDocumentRequest>()
                .HasMany(r => r.History)
                .WithOne(h => h.Request)
                .HasForeignKey(h => h.RequestId);

            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Requests)
                .WithOne()
                .HasForeignKey(r => r.ApplicantNID)
                .HasPrincipalKey(u => u.NID);
        }
    }
}
