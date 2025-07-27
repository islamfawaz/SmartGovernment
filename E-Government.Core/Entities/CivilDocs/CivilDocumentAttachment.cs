using System;

namespace E_Government.Domain.Entities.CivilDocs
{
    public class CivilDocumentAttachment
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadedAt { get; set; }

        public CivilDocumentRequest Request { get; set; }
    }
}