using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.License
{
    public class TrafficViolationDTO
    {
        [Required]
        public string PlateNumber { get; set; }
        [Required]
        public string ViolationType { get; set; }
        [Required]
        public decimal FineAmount { get; set; }
        [Required]
        public DateTime ViolationDate { get; set; }
        [Required]
        public string PaymentStatus { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        [Required]
        public string PaymentReceipt { get; set; }
    }
}
