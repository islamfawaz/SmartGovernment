using E_Government.Application.DTO.User;
using E_Government.Domain.Entities;
using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.Liscenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.License
{
    public class LicenseRequestDto
    {

        public Guid Id { get; set; }
        public string LicenseType { get; set; }
        public string ServiceCode { get; set; }

        public string ApplicantNID { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string ? Status { get; set; }
        public string ? Notes { get; set; }

        public string? UploadedDocumentUrl { get; set; }
        public Dictionary<string, string> ? ExtraFields { get; set; }
    }
}
