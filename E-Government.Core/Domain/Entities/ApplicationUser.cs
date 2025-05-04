using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_Government.Core.Domain.Entities
{
    public class ApplicationUser :IdentityUser<string>
    {
         public string NID { get; set; }        //public string State { get; set; }
        public string  Address { get; set; }
     //   public string AccountNumber { get; set; } // New property from JSON
        public CustomerCategory Category { get; set; } = CustomerCategory.Residential;
        //public CustomerStatus Status { get; set; } = CustomerStatus.Active;

        // Navigation properties
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<Meter> Meters { get; set; } = new List<Meter>();
        public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    }

}