using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using E_Government.UI.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Billing;

namespace E_Government.UI.Controllers.Bills
{
    public class BillsController:ApiControllerBase
    {
        private readonly IBillingService _billingService;

        public BillsController(IBillingService billingService)
        {
            _billingService = billingService;
        }


        [HttpPost("register-meter")]
        public async Task<IActionResult> RegisterMeter([FromBody] RegisterMeterDto request)
        {
            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Call service and get the result
            var result = await _billingService.RegisterMeter(request);

            // Handle the result
            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "CUSTOMER_NOT_FOUND" => NotFound(new
                    {
                        result.ErrorMessage,
                        result.ErrorCode
                    }),
                    _ => StatusCode(500, new
                    {
                        result.ErrorMessage,
                        result.ErrorCode
                    })
                };
            }

            // Success case - map to API response
            return Ok(new
            {
                MeterId = result.Meter.Id,
                MeterNumber = result.Meter.MeterNumber,
                Type = result.Meter.Type.ToString(),
                InstallationDate = result.Meter.InstallationDate,
                Message = "Meter registered successfully"
            });
        }



        [HttpPost("generate-and-pay")]
        public async Task<IActionResult> GenerateAndPayBill([FromBody] GenerateBillRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           
                var result = await _billingService.GenerateAndPayBill(request);

                if (!result.Success)
                {
                    return result.ErrorMessage switch
                    {
                        string msg when msg.Contains("Customer with ID") => NotFound(new
                        {
                            result.ErrorMessage
                        }),
                        string msg when msg.Contains("No meter found") => BadRequest(new
                        {
                            result.ErrorMessage
                        }),
                        _ => StatusCode(500, new
                        {
                            result.ErrorMessage
                        })
                    };
                }

                return Ok(new
                {
                    Success = true,
                    BillNumber = result.BillNumber,
                    Amount = result.Amount,
                    PaymentIntentId = result.PaymentIntentId,
                    ClientSecret = result.ClientSecret,
                    Message = "Bill generated and payment intent created successfully"
                });
            
          
        }
    }
}
