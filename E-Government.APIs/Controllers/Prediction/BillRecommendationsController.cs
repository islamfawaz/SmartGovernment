using E_Government.APIs.Controllers.Base;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities.DataModels;
using E_Government.Domain.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace E_Government.APIs.Controllers.Prediction
{
    public class BillRecommendationsController :ApiControllerBase
    {
        private readonly IModelService service;

        public BillRecommendationsController(IModelService service)
        {
            this.service = service;
        }

        [HttpPost("analyze")] 
        public async Task<IActionResult> AnalyzeBill([FromBody] BillData billData)
        {
            if (billData == null)
            {
                return BadRequest("بيانات الفاتورة مطلوبة");
            }

            var recommendations = await service.GetRecommendationsAsync(billData);
            return Ok(recommendations);
        }
    }
}
