using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.License
{
    public class LicenseReplacementDto
    {
        [Required]

        public string LicenseType { get; set; }
        [Required]

        public string OriginalLicenseNumber { get; set; }
        [Required]

        public string Reason { get; set; } // Lost or Damaged
        [Required]

        public string PoliceReport { get; set; }
        [Required]

        public string DamagedLicensePhoto { get; set; }
        [Required]

        public decimal ReplacementFee { get; set; }
        [Required]

        public string PaymentMethod { get; set; }
    }
}
