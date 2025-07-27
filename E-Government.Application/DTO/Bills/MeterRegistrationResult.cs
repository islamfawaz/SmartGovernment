using E_Government.Domain.Entities.Bills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.Bills
{
    public class MeterRegistrationResult
    {
        public bool Success { get; set; }
        public Meter Meter { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
    }
}
