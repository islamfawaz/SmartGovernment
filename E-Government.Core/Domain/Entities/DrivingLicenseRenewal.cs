using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Government.Core.Domain.Entities
{
    public class DrivingLicenseRenewal
    {
        public int Id { get; set; }
        //        Current License Number
        public int CurrentLicenseNumber { get; set; }

//National ID
public int NID { get; set; }
//Current Expiry Date
public DateOnly CurrentExpiryDate { get; set; }
//Medical Checkup Required(Yes/No)
public string MedicalCheckRequired { get; set; }
//New Photo
public string NewPhoto {  get; set; }
//Payment Method
public string PaymentMethod { get; set; }
//Renewal Date
public string RenewalDate { get; set; }
//New Expiry Date

        public string NewExpirayDate { get; set; }
    }
}
