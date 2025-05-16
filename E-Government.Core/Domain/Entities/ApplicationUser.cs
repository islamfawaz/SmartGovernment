using E_Government.Core.Domain.Entities.CivilDocs;
using E_Government.Core.Domain.Entities.Liscenses;
using E_Government.Core.DTO;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_Government.Core.Domain.Entities
{
    public class ApplicationUser :IdentityUser
    {
         public string NID { get; set; }      
        public string  Address { get; set; }
         public CustomerCategory Category { get; set; } = CustomerCategory.Residential;

        public string DisplayName { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

        public virtual ICollection<CivilDocumentRequest> Requests { get; set; } = new List<CivilDocumentRequest>();

        public virtual ICollection<DrivingLicenseRenewal> DrivingLicenseRenewals { get; set; } = new List<DrivingLicenseRenewal>();

        public virtual ICollection<LicenseReplacementRequest> LicenseReplacementRequests { get; set; } = new List<LicenseReplacementRequest>();

        public virtual ICollection<VehicleLicenseRenewal> VehicleLicenseRenewals { get; set; } = new List<VehicleLicenseRenewal>();

        public virtual ICollection<VehicleOwner> VehicleOwners { get; set; } = new List<VehicleOwner>();




    }

}