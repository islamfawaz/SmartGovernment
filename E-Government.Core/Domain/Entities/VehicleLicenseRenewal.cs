using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities
{
    public class VehicleLicenseRenewal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string PlateNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string VehicleRegistrationNumber { get; set; }

        public string TechnicalInspectionReport { get; set; }

        public string InsuranceDocument { get; set; }

        // Null means no pending fines
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PendingFines { get; set; }

        [MaxLength(30)]
        public string PaymentMethod { get; set; }

        public DateTime? RenewalDate { get; set; }
    }
}
