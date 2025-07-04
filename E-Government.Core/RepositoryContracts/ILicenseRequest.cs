using E_Government.Domain.Entities;

namespace E_Government.Domain.RepositoryContracts
{
    public interface ILicenseRequest
    {
        ApplicationUser Applicant { get; set; } // ⬅️ أضف set
        Guid PublicId { get; }

        string ApplicantNID { get; set; }       // ⬅️ أضف set
        DateTime? RequestDate { get; set; }     // ⬅️ أضف set
        string Status { get; set; }             // ⬅️ أضف set
        string? Notes { get; set; }             // ⬅️ أضف property جديدة
        DateTime? LastUpdated { get; set; }     // ⬅️ أضف property جديدة

        string ApplicantName => Applicant?.UserName ?? Applicant?.Email ?? "Unknown";
    }
}
