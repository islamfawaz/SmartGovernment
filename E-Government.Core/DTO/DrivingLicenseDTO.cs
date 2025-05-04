using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class DrivingLicenseDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int NID { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; } = new DateOnly();
        [Required]
        public string Address { get; set; }
        [Required]
        public string LicenseType { get; set; }
        [Required]
        public IFormFile photo { get; set; }
        [Required]
        public string MedicalTest { get; set; }
        [Required]
        public string TheoryTest { get; set; }
        [Required]
        public string PracticalTest { get; set; }
        [Required]
        public DateOnly IssueDate { get; set; } = new DateOnly();
        [Required]
        public DateOnly ExpiryDate { get; set; } = new DateOnly();
    }
}
