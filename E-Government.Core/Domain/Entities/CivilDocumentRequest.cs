using System;
using System.Collections.Generic;

namespace E_Government.Core.Domain.Entities
{
    public class CivilDocumentRequest
    {
        public Guid Id { get; set; }
        public string DocumentType { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantNID { get; set; }
        public string Relation { get; set; }
        public string OwnerName { get; set; }
        public string OwnerNID { get; set; }
        public string OwnerMotherName { get; set; }
        public int CopiesCount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string ExtraFieldsJson { get; set; } // Stores ExtraFields as JSON

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public Dictionary<string, string> ExtraFields
        {
            get => string.IsNullOrEmpty(ExtraFieldsJson)
                ? new Dictionary<string, string>()
                : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(ExtraFieldsJson)!;
            set => ExtraFieldsJson = System.Text.Json.JsonSerializer.Serialize(value);
        }

        public ICollection<CivilDocumentAttachment> Attachments { get; set; }
        public ICollection<CivilDocumentRequestHistory> History { get; set; }
    }
}