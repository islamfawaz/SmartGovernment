using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities
{
    public class TrafficViolationPayment
    {
        [Key]
        public int ViolationNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string PlateNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string ViolationType { get; set; } // e.g., "Red Light", "Speeding", etc.

        [Column(TypeName = "decimal(18,2)")]
        public decimal FineAmount { get; set; }

        public DateTime ViolationDate { get; set; }

        [Required]
        [MaxLength(10)]
        public string PaymentStatus { get; set; } // "Paid" or "Unpaid"

        [MaxLength(30)]
        public string PaymentMethod { get; set; }

        public string PaymentReceipt { get; set; }
    }
}
