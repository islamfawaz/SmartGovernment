using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.CivilDocs;
using E_Government.Domain.Entities.Enums;
using E_Government.Domain.Entities.Liscenses;
using Microsoft.AspNetCore.Identity;

namespace E_Government.Domain.Entities
{
    public class ApplicationUser :IdentityUser
    {
         public required string NID { get; set; }      
        public string ? Address { get; set; }
         public CustomerCategory Category { get; set; } = CustomerCategory.Residential;

        public Gender Gender { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public int GovernorateCode { get; set; }
        public string ? GovernorateName { get; set; } 

        public string ? DisplayName { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

        public virtual ICollection<CivilDocumentRequest> Requests { get; set; } = new List<CivilDocumentRequest>();

        public virtual ICollection<DrivingLicenseRenewal> DrivingLicenseRenewals { get; set; } = new List<DrivingLicenseRenewal>();

        public virtual ICollection<LicenseReplacementRequest> LicenseReplacementRequests { get; set; } = new List<LicenseReplacementRequest>();

        public virtual ICollection<VehicleLicenseRenewal> VehicleLicenseRenewals { get; set; } = new List<VehicleLicenseRenewal>();

        public virtual ICollection<VehicleOwner> VehicleOwners { get; set; } = new List<VehicleOwner>();

    }

}