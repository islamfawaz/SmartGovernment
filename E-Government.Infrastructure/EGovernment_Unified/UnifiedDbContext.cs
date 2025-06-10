using E_Government.Domain.Entities;
using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.CivilDocs;
using E_Government.Domain.Entities.Liscenses;
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

        public DbSet<Meter> Meters { get; set; }
        public DbSet<Bill> Bills { get; set; }
     //   public DbSet<DrivingLicense> DrivingLicenses { get; set; }
        public DbSet<DrivingLicenseRenewal> DrivingLicenseRenewals { get; set; }
        public DbSet<VehicleLicenseRenewal> VehicleLicenseRenewals { get; set; }

        public DbSet<LicenseRequest> LicenseRequests { get; set; }
        public DbSet<TrafficViolationPayment> TrafficViolationPayments { get; set; }

        public DbSet<DrivingLicense> DrivingLicenses { get; set; }
        public DbSet<LicenseReplacementRequest> LicenseReplacementRequests { get; set; }


        public DbSet<VehicleOwner> VehicleOwners { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<MeterReading> MeterReadings { get; set; }

        public DbSet<CivilDocumentRequest> CivilDocumentRequests { get; set; }
        public DbSet<CivilDocumentAttachment> CivilDocumentAttachments { get; set; }
        public DbSet<CivilDocumentRequestHistory> CivilDocumentRequestHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Call base first to get default Identity configurations

            // Apply your specific entity configurations, which includes setting NID as PK for ApplicationUser
            builder.ApplyConfigurationsFromAssembly(typeof(UnifiedDbContext).Assembly);

            // --- Explicitly re-configure IdentityUserRole to use NID as the principal key for UserId ---
            builder.Entity<IdentityUserRole<string>>(b =>
            {
                // The composite primary key for IdentityUserRole (UserId, RoleId) is standard.
                // b.HasKey(ur => new { ur.UserId, ur.RoleId }); // This is usually handled by base.OnModelCreating

                // Configure the relationship from IdentityUserRole to ApplicationUser
                b.HasOne<ApplicationUser>() // Specifies the principal entity
                    .WithMany()             // Assumes ApplicationUser doesn't have a direct ICollection<IdentityUserRole<string>> navigation property for roles (which is typical)
                    .HasForeignKey(ur => ur.UserId) // The foreign key property in IdentityUserRole<string>
                    .HasPrincipalKey(u => u.NID);   // CRUCIAL: Tells EF Core that ur.UserId maps to ApplicationUser.NID

                // The relationship from IdentityUserRole to IdentityRole usually remains standard
                // b.HasOne<IdentityRole>()
                //    .WithMany()
                //    .HasForeignKey(ur => ur.RoleId)
                //    .HasPrincipalKey(r => r.Id); // This is usually handled by base.OnModelCreating
            });

            // IMPORTANT: You may need to do similar explicit configurations for other Identity join tables
            // if you use their features and they also have foreign keys to ApplicationUser:
            // builder.Entity<IdentityUserClaim<string>>(b =>
            // {
            //     b.HasOne<ApplicationUser>().WithMany().HasForeignKey(uc => uc.UserId).HasPrincipalKey(u => u.NID);
            // });
            // builder.Entity<IdentityUserLogin<string>>(b =>
            // {
            //     b.HasOne<ApplicationUser>().WithMany().HasForeignKey(ul => ul.UserId).HasPrincipalKey(u => u.NID);
            // });
            // builder.Entity<IdentityUserToken<string>>(b =>
            // {
            //     b.HasOne<ApplicationUser>().WithMany().HasForeignKey(ut => ut.UserId).HasPrincipalKey(u => u.NID);
            // });

            // Your existing custom configurations (these seem fine)
            builder.Entity<CivilDocumentRequest>()
                .HasMany(r => r.Attachments)
                .WithOne(a => a.Request)
                .HasForeignKey(a => a.RequestId);

            builder.Entity<CivilDocumentRequest>()
                .HasMany(r => r.History)
                .WithOne(h => h.Request)
                .HasForeignKey(h => h.RequestId);

            // This custom relationship already correctly uses NID
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Requests)
                .WithOne()
                .HasForeignKey(r => r.ApplicantNID)
                .HasPrincipalKey(u => u.NID);
        }

    }
}
