using E_Government.Domain.Entities.Bills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.Entities.Liscenses
{
    public class LicenseRequest
    {

        public Guid Id { get; set; } = Guid.NewGuid();
        public string LicenseType { get; set; }
        public string ServiceCode { get; set; }

        public DateTime ? CreatedAt { get; set; } = DateTime.UtcNow;
        public string ApplicantNID { get; set; }

        public string ApplicantName { get; set; }

        public DateTime RequestDate { get; set; }= DateTime.UtcNow;
        public string Status { get; set; } // Pending, Approved, Rejected

        public string? Notes { get; set; }
        public string? UploadedDocumentUrl { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public Bill Bill { get; set; }

        public required string ExtraFieldsJson { get; set; } // Stores ExtraFields as JSON


        [NotMapped]
        public Dictionary<string, string> ExtraFields
        {
            get => string.IsNullOrEmpty(ExtraFieldsJson)
                ? new Dictionary<string, string>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(ExtraFieldsJson)!;
            set => ExtraFieldsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        public virtual ICollection<LicenseRequestHistory> LicenseRequestHistories { get; set; }=new HashSet<LicenseRequestHistory>();

    }
}
