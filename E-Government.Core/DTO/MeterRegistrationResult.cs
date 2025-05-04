using E_Government.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class MeterRegistrationResult
    {
        public bool Success { get; set; }
        public Meter Meter { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
    }
}
