using E_Government.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities.Liscenses
{
    public class LicenseRequest : ILicenseRequest
    {
        public ApplicationUser Applicant { get; set; }
        [Key]
        public Guid PublicId { get; set; }

        public string ApplicantNID { get; set; }

        public DateTime RequestDate { get;set; }

        public string Status { get;set; }
    }
}
