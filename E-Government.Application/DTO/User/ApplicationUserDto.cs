using E_Government.Application.CivilDocs;
using E_Government.Application.DTO.Bills;
using E_Government.Domain.DTO;
using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.Enums;

namespace E_Government.Application.DTO.User
{
    public class ApplicationUserDto
    {
        public string NID { get; set; }      
        public string Address { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }

        public string DisplayName { get; set; }
        public CustomerCategory Category { get; set; } = CustomerCategory.Residential;
        public Gender Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int GovernorateCode { get; set; }
        public GovernorateDto ? GovernorateName { get; set; }

        // Navigation properties
        public MetersDto Meters { get; set; }
        public BillDto Bills { get; set; }
        public  CivilDocsDto Requests { get; set; 
        
        } 


    }
}
