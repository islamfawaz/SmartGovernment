using E_Government.Domain.RepositoryContracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Government.Domain.Entities.Liscenses
{
    public class LicenseReplacementRequest  :ILicenseRequest
    {
        [Key]
        public int Id { get; set; } // Internal DB ID

        public Guid PublicId { get; set; } = Guid.NewGuid(); // Public facing ID

        [Required]
        public string ApplicantNID { get; set; }
        [ForeignKey("ApplicantNID")]
        public ApplicationUser Applicant { get; set; }


        [Required]
        [MaxLength(20)]
        public string LicenseType { get; set; } // e.g., "Driver's" or "Vehicle"

        [Required]
        [MaxLength(50)]
        public string OriginalLicenseNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string Reason { get; set; } // e.g., "Lost" or "Damaged"

        public string? PoliceReport { get; set; } // Path/URL, Required if Lost, nullable

        public string? DamagedLicensePhoto { get; set; } // Path/URL, Required if Damaged, nullable

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReplacementFee { get; set; }

        [MaxLength(30)]
        public string PaymentMethod { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } // Date of the replacement request

        [Required]
        public string Status { get; set; } // e.g., "Pending", "Approved", "Rejected"

        public string? Notes { get; set; } // Admin notes, nullable

        public DateTime LastUpdated { get; set; }
    }
}
