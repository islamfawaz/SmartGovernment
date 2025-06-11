using E_Government.Application.DTO.User;
using E_Government.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts
{
    public interface INIDValidationService
    {
        public NIDValidationResultDto ValidateAndExtractNID(string NID);

       bool ValidateNID(string NID);

       DateOnly ExtractDateOfBirth(string NID);

       Gender ExtractGender(string NID);

        GovernorateDto ExtractGovernorateInfo(string NID);

      //  int CalculateCheckDigit(string NID);

    }
}
