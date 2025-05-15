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




    }

}