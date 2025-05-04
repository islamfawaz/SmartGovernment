using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class RenewDrivingDTO
    {
        [Required]
        public int CurrentLicenseNumber { get; set; }
        [Required]
        //National ID
        public int NID { get; set; }
        [Required]
        //Current Expiry Date
        public DateOnly CurrentExpiryDate { get; set; }
        [Required]
        //Medical Checkup Required(Yes/No)
        public string MedicalCheckRequired { get; set; }
        [Required]
        //New Photo
        public IFormFile NewPhoto { get; set; }
        [Required]
        //Payment Method
        public string PaymentMethod { get; set; }
        [Required]
        //Renewal Date
        public string RenewalDate { get; set; }
        [Required]
        //New Expiry Date

        public string NewExpirayDate { get; set; }
    }
}
