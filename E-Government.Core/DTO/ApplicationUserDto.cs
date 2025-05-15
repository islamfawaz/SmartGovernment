using E_Government.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class ApplicationUserDto
    {
        public string NID { get; set; }      
        public string Address { get; set; }

        public string Token { get; set; }

        public string DisplayName { get; set; }
        public CustomerCategory Category { get; set; } = CustomerCategory.Residential;

        // Navigation properties
        public  MetersDto Meters { get; set; }
        public BillDto Bills { get; set; }
        public  CivilDocsDto Requests { get; set; 
        
        } 


    }
}
