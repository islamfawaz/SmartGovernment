using E_Government.Core.Domain.Entities;
using E_Government.Core.DTO;

namespace E_Government.Core.ServiceContracts
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
