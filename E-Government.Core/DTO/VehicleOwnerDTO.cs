using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class VehicleOwnerDTO
    {
        [Required]
        public string NationalId { get; set; }
        [Required]
        public string OwnerName { get; set; }
        [Required]
        public string VehicleType { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int ManufactureYear { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public string ChassisNumber { get; set; }
        [Required]
        public string InspectionReport { get; set; }
        [Required]
        public string InsuranceDocument { get; set; }
        [Required]
        public string OwnershipProof { get; set; }
    }
}
