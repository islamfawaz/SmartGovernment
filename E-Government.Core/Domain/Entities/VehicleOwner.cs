using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities
{
    public class VehicleOwner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string NationalId { get; set; }

        [Required]
        [MaxLength(100)]
        public string OwnerName { get; set; }

        // Vehicle Details
        [Required]
        [MaxLength(20)]
        public string VehicleType { get; set; } // Car/Truck/etc.

        [MaxLength(50)]
        public string Model { get; set; }

        public int ManufactureYear { get; set; }

        [MaxLength(30)]
        public string Color { get; set; }

        [MaxLength(100)]
        public string ChassisNumber { get; set; }

        public string InspectionReport { get; set; }

        public string InsuranceDocument { get; set; }

        public string OwnershipProof { get; set; }
    }
}
