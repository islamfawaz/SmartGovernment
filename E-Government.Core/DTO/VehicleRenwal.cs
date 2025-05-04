using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class VehicleRenwal
    {
        [Required]
        public string PlateNumber { get; set; }
        [Required]
        public string VehicleRegistrationNumber { get; set; }
        [Required]
        public string TechnicalInspectionReport { get; set; }
        [Required]
        public string InsuranceDocument { get; set; }
        [Required]
        public decimal? PendingFines { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public DateTime? RenewalDate { get; set; }
    }
}
