using System;
using System.Collections.Generic;

namespace E_Government.Application.DTO.CivilDocs
{
    public class CivilDocumentRequestDto
    {
        public Guid? Id { get; set; }
        public string DocumentType { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantNID { get; set; }
        public string Relation { get; set; }
        public string OwnerName { get; set; }
        public string OwnerNID { get; set; }
        public string OwnerMotherName { get; set; }
        public int CopiesCount { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<CivilDocumentAttachmentDto> Attachments { get; set; } = new();
        public List<CivilDocumentRequestHistoryDto> History { get; set; } = new();
        public Dictionary<string, string> ExtraFields { get; set; }
    }
} 