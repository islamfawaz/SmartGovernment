using E_Government.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.User
{
    public class NIDValidationResultDto
    {
        public bool IsValid { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public GovernorateDto? Governorate { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
