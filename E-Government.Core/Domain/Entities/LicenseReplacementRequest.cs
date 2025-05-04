using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities
{
    public class LicenseReplacementRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string LicenseType { get; set; } // "Driver's" or "Vehicle"

        [Required]
        [MaxLength(50)]
        public string OriginalLicenseNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string Reason { get; set; } // "Lost" or "Damaged"

        public string PoliceReport { get; set; } // Required if Lost

        public string DamagedLicensePhoto { get; set; } // Required if Damaged

        [Column(TypeName = "decimal(18,2)")]
        public decimal ReplacementFee { get; set; }

        [MaxLength(30)]
        public string PaymentMethod { get; set; }
    }
}
