using E_Government.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class RegisterDTO
    {
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public string NID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }
        public CustomerCategory Category { get; set; } = CustomerCategory.Residential;

        public string PhoneNumber { get; set; }
    }
}
