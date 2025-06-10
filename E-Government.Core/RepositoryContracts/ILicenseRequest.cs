using E_Government.Domain.Entities;

namespace E_Government.Domain.RepositoryContracts
{
    public interface ILicenseRequest
    {
        public ApplicationUser Applicant { get;  }
        public Guid PublicId { get; } 
         string ApplicantNID { get; }
        DateTime RequestDate { get; }
        string Status { get; }
    }
}
