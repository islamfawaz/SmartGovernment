using System;
using System.Collections.Generic;

namespace E_Government.Domain.Entities.CivilDocs
{
    public class CivilDocumentRequest
    {
        public Guid Id { get; set; }
        public required string DocumentType { get; set; }
        public required string ApplicantName { get; set; }
        public required string ApplicantNID { get; set; }
        public required string Relation { get; set; }
        public required string OwnerName { get; set; }
        public required string OwnerNID { get; set; }
        public required string OwnerMotherName { get; set; }
        public int CopiesCount { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }

        #region Receive Data
        public required string Governorate { get; set; }
        public required string District { get; set; }
        public  required string City { get; set; }
        public required string DetailsAddress { get; set; }
        #endregion
        public required string ExtraFieldsJson { get; set; } // Stores ExtraFields as JSON
       

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