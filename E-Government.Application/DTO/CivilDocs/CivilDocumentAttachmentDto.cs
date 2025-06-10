using System;

namespace E_Government.Application.DTO.CivilDocs
{
    public class CivilDocumentAttachmentDto
    {
        public Guid? Id { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public DateTime? UploadedAt { get; set; }
    }
} 