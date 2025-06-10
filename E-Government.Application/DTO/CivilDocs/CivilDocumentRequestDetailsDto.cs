using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.CivilDocs
{
    public class CivilDocumentRequestDetailsDto // Maps to CivilDocumentRequest entity/DTO
    {
        public Guid Id { get; set; }
        public string ApplicantNID { get; set; }
        public string ApplicantName { get; set; }
        public string DocumentType { get; set; } // e.g., BirthCertificate, MarriageCertificate
        public DateTime RequestDate { get; set; }
        public string CurrentStatus { get; set; }
        public string Notes { get; set; }
        public List<CivilDocumentAttachmentDto> Attachments { get; set; }
        public List<RequestHistoryDto> History { get; set; }
    }
}
