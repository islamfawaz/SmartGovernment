using E_Government.Application.DTO.AdminDashBoard;
using E_Government.Domain.Entities.DataModels;

namespace E_Government.APIs.Controllers.Prediction
{
    public interface IBillPredictionService
    {
        Task<BillRecommendationDto> GetRecommendationsAsync(BillData billData);
        Task<bool> IsBillHighAsync(BillData billData);
    }
}