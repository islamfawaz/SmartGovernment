using E_Government.Core.Domain.Entities.DataModels;
using E_Government.Core.DTO;

namespace E_Government.Core.ServiceContracts
{
   public interface IBillPredictionService
    {
        Task<BillRecommendationDto> GetRecommendationsAsync(BillData billData);
        Task<bool> IsBillHighAsync(BillData billData);
    }
}
