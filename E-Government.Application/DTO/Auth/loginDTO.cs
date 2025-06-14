using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.Auth
{
    public class loginDTO
    {    
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
