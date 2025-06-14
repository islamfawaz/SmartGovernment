using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.Entities.OTP
{
    public class OtpCode
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public string Purpose { get; set; } = string.Empty; //Register, ResetPassword

        public ApplicationUser? User { get; set; }
    }
}
