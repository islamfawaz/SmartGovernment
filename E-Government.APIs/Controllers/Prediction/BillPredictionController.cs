using E_Government.APIs.Controllers.Base;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities;
using E_Government.Domain.Entities.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace E_Government.APIs.Controllers.Prediction
{

    public class BillPredictionController : ApiControllerBase
    {
        private readonly IPredictionService _predictionService;

        public BillPredictionController(IPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost("predict")]
        public IActionResult Predict([FromBody] BillPredictionRequestDto request)
        {
            if (request == null)
                return BadRequest("Input data is required.");

            var billData = new BillData
            {
                BillAmount = request.BillAmount,
                Consumption = request.Consumption,
                BillMonth = request.BillMonth,
                BillYear = request.BillYear,
                DaysInBillingCycle = request.DaysInBillingCycle,
                MeterType = request.MeterType,
                NumberOfAirConditioners = request.NumberOfAirConditioners,
                AirConditionerUsageHours = request.AirConditionerUsageHours,
                AirConditionerType = request.AirConditionerType,
                NumberOfLights = request.NumberOfLights,
                LightType = request.LightType,
                LightUsageHours = request.LightUsageHours,
                OtherMajorAppliances_Count = request.OtherMajorAppliances_Count,
                ApplianceUsage_Encoded = request.ApplianceUsage_Encoded,
                HouseholdSize = request.HouseholdSize,
                HomeType_Encoded = request.HomeType_Encoded,
                ConsumptionTrend = request.ConsumptionTrend,
                SeasonalConsumptionPattern = request.SeasonalConsumptionPattern
            };

            var result = _predictionService.Predict(billData);

            return Ok(new
            {
                Prediction = result.PredictedLabel,
                Confidence = result.Probability,
                Score = result.Score
            });
        }
    }
}

