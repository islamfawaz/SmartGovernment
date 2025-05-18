using E_Government.APIs.Controllers.Base;
using E_Government.Core.Domain.Entities.DataModels;
using E_Government.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace E_Government.APIs.Controllers.Prediction
{
    public class BillRecommendationsController :ApiControllerBase
    {
        private readonly IBillPredictionService _predictionService;
                                                                                                                                    
        public BillRecommendationsController(IBillPredictionService predictionService)
        {
           _predictionService = predictionService;
        }

        [HttpPost("analyze")] 
        public async Task<IActionResult> AnalyzeBill([FromBody] BillData billData)
        {
            if (billData == null)
            {
                return BadRequest("بيانات الفاتورة مطلوبة");
            }

            var recommendations = await _predictionService.GetRecommendationsAsync(billData);
            return Ok(recommendations);
        }
    }
}
