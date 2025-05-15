using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Government.Core.Domain.Entities.Liscenses
{
    public class DrivingLicenseRenewal
    {
        [Key]
        public int Id { get; set; } // Internal DB ID

        public int NID { get; set; }

        public string NewExpirayDate { get; set; }


        public Guid PublicId { get; set; } = Guid.NewGuid(); // Public facing ID

        [Required]
        public string ApplicantNID { get; set; }

        [Required]
        public string ApplicantName { get; set; }

        public int CurrentLicenseNumber { get; set; }

        public DateOnly CurrentExpiryDate { get; set; }

        public string MedicalCheckRequired { get; set; } // e.g., "Yes", "No", "Pending"

        public string? NewPhoto { get; set; } // Path or URL to new photo, nullable

        public string PaymentMethod { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } // Date of the renewal request

        public DateOnly? NewExpiryDate { get; set; } // Nullable, set upon approval
        public DateTime RenewalDate { get; set; }



        [Required]
        public string Status { get; set; } // e.g., "Pending", "Approved", "Rejected"

        public string? Notes { get; set; } // Admin notes, nullable

        public DateTime LastUpdated { get; set; }
    }
}
