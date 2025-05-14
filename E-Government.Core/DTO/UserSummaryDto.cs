using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class UserSummaryDto // Aligned with ApplicationUser.cs / ApplicationUserDto.cs
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string NID { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public List<string> Roles { get; set; }
        public bool IsActive { get; set; }
    }
}
