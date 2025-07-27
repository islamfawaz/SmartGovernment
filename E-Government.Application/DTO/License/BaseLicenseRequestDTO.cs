using E_Government.Application.DTO.User;
using E_Government.Domain.Entities.Bills;
using System.Text.Json.Serialization;

namespace E_Government.Application.DTO.License
{
    public class BaseLicenseRequestDTO
    {
        public int Id { get; set; }

        public Guid PublicId { get; set; } = Guid.NewGuid();

        public string ApplicantNID { get; set; }

        public ApplicationUserDto Applicant { get; set; }

        public DateTime RequestDate { get; set; }

        public string Status { get; set; } // Pending, Approved, Rejected

        public string? Notes { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public int? BillId { get; set; }
        [JsonIgnore] 
        public Bill Bill { get; set; }
        public string ServiceCode { get; set; }
    }
}
