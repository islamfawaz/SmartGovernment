
using E_Government.Domain.RepositoryContracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Government.Domain.Entities.Liscenses
{
    public class DrivingLicenseRenewal  :ILicenseRequest

    {
        public Guid PublicId { get; set; } = Guid.NewGuid();
        [Required]
        public string ApplicantNID { get; set; }
        [ForeignKey("ApplicantNID")]
        public ApplicationUser Applicant { get; set; }

        [Required]

        public DateTime RequestDate { get; set; }
        [Required]
        public string Status { get; set; }



        [Key]
        public int Id { get; set; } // Internal DB ID

        public int NID { get; set; }

        public string NewExpirayDate { get; set; }





        public int CurrentLicenseNumber { get; set; }

        public DateOnly CurrentExpiryDate { get; set; }

        public string MedicalCheckRequired { get; set; } // e.g., "Yes", "No", "Pending"

        public string? NewPhoto { get; set; } // Path or URL to new photo, nullable

        public string PaymentMethod { get; set; }


        public DateOnly? NewExpiryDate { get; set; } // Nullable, set upon approval
        public DateTime RenewalDate { get; set; }




        public string? Notes { get; set; } // Admin notes, nullable

        public DateTime LastUpdated { get; set; }
    }
}
