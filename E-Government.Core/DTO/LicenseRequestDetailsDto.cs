using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class LicenseRequestDetailsDto // Maps to DrivingLicense entity/DTO
    {
        public Guid Id { get; set; }
        public string ApplicantNID { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantAddress { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string LicenseType { get; set; } // e.g., Private, Professional
        public string PhotoUrl { get; set; } // Assuming IFormFile is handled and URL is stored
        public string MedicalTestStatus { get; set; }
        public string TheoryTestStatus { get; set; }
        public string PracticalTestStatus { get; set; }
        public DateTime RequestDate { get; set; }
        public string CurrentStatus { get; set; }
        public string LicenseNumber { get; set; } // If issued
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<RequestHistoryDto> History { get; set; }
    }
}
