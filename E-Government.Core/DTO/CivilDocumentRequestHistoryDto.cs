using System;

namespace E_Government.Core.DTO
{
    public class CivilDocumentRequestHistoryDto
    {
        public Guid? Id { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
} 