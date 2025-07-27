using E_Government.Application.DTO.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.OTP
{
    public class OtpResponseDTO
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public ApplicationUserDto? User { get; set; }
    }
}
