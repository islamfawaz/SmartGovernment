using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.CivilDocs;
using E_Government.Domain.Entities.Enums;
using E_Government.Domain.Entities.Liscenses;
using E_Government.Domain.Entities.OTP;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace E_Government.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public required string NID { get; set; }
        public string? Address { get; set; }
        public Status Status { get; set; }
        public CustomerCategory CustomerCategory { get; set; } = CustomerCategory.Residential;
        public Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int GovernorateCode { get; set; }
        public string? GovernorateName { get; set; }
        public string? DisplayName { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();
        public virtual ICollection<CivilDocumentRequest> Requests { get; set; } = new List<CivilDocumentRequest>();

        public virtual ICollection<LicenseRequest> LicenseRequests { get; set; }
        public virtual ICollection<OtpCode> OtpCodes { get; set; } = new HashSet<OtpCode>();

        // Constructor to ensure NID and Id are synchronized
        public ApplicationUser()
        {
        }

        public ApplicationUser(string nid)
        {
            NID = nid;
            Id = nid; // الـ Id والـ NID لازم يكونوا نفس القيمة
        }

        // Method عشان تضمن إن الـ Id والـ NID متزامنين دايماً  
        public void SetNID(string nid)
        {
            NID = nid;
            Id = nid;
        }

        // Override عشان لما حد يحط قيمة في الـ Id يتحط في الـ NID كمان
        public override string Id
        {
            get => base.Id;
            set
            {
                base.Id = value;
                if (!string.IsNullOrEmpty(value) && NID != value)
                {
                    NID = value;
                }
            }
        }
    }
}