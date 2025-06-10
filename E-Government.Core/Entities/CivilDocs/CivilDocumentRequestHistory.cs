using System;

namespace E_Government.Domain.Entities.CivilDocs
{
    public class CivilDocumentRequestHistory
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime ChangedAt { get; set; }

        public CivilDocumentRequest Request { get; set; }
    }
} 