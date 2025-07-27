using E_Government.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.OTP
{
    public class RegisterWithOtpDTO :RegisterDTO
    {
        public string OtpCode { get; set; } = string.Empty;

    }
}
