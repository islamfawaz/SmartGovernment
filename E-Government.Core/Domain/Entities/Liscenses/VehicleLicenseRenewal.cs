using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities.Liscenses
{
    public class VehicleLicenseRenewal
    {
        [Key]
        public int Id { get; set; } // Internal DB ID

        public Guid PublicId { get; set; } = Guid.NewGuid(); // Public facing ID

        [Required]
        public string ApplicantNID { get; set; }

        [Required]
        public string ApplicantName { get; set; }

        [Required]
        [MaxLength(20)]
        public string PlateNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string VehicleRegistrationNumber { get; set; }

        public string? TechnicalInspectionReport { get; set; } // Path/URL, nullable

        public string? InsuranceDocument { get; set; } // Path/URL, nullable

        [Column(TypeName = "decimal(18,2)")]
        public decimal? PendingFines { get; set; } // Null means no pending fines

        [MaxLength(30)]
        public string PaymentMethod { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } // Date of the renewal request, changed from RenewalDate nullable

        [Required]
        public string Status { get; set; } // e.g., "Pending", "Approved", "Rejected"

        public string? Notes { get; set; } // Admin notes, nullable

        public DateTime LastUpdated { get; set; }

        public DateTime? RenewalDate { get; set; }

    }
}
