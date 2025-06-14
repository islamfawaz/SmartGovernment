using E_Government.Application.CivilDocs;
using E_Government.Application.DTO.Bills;
using E_Government.Domain.DTO;
using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.Enums;

namespace E_Government.Application.DTO.User
{
    public class ApplicationUserDto
    {
        public required string NID { get; set; }      
        public string ? Address { get; set; }

        public required string Email { get; set; }
        public  required string Token { get; set; }

        public required string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public Status Status { get; set; }
        public Gender Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int GovernorateCode { get; set; }
        public GovernorateDto ? GovernorateName { get; set; }

        // Navigation properties
        public MetersDto Meters { get; set; }
        public BillDto Bills { get; set; }
        public  CivilDocsDto Requests { get; set; 
        
        } 


    }
}
