using E_Government.Application.DTO.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.License
{
    public class VehicleOwnerDTO
    {
        public string NationalId { get; set; }

        public ApplicationUserDto User { get; set; }
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
