using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.Entities.OTP
{
    public class SendOtpDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Purpose { get; set; } = "Register";
    }
}
