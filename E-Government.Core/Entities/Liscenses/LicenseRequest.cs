using E_Government.Domain.RepositoryContracts;
using System.ComponentModel.DataAnnotations;

namespace E_Government.Domain.Entities.Liscenses
{
    public class LicenseRequest : ILicenseRequest
    {
        public ApplicationUser Applicant { get; set; }
        [Key]
        public Guid PublicId { get; set; }

        public string ApplicantNID { get; set; }

        public DateTime RequestDate { get;set; }

        public string Status { get;set; }
    }
}
