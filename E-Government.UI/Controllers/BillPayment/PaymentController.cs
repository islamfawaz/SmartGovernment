using E_Government.Core.Domain.RepositoryContracts.Infrastructure;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using E_Government.UI.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.IO;
using System.Threading.Tasks;

namespace E_Government.UI.Controllers.BillsPayment
{
    [Route("api/[controller]")]

    public class PaymentController : ApiControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("initiate-payment")]
        public async Task<IActionResult> InitiateBillPayment([FromBody] BillPaymentRequest request)
        {
            try
            {
                var result = await _paymentService.CreatePaymentIntent(request);

                if (!result.Success)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred while processing your payment" });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> HandlePaymentWebhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();

                if (string.IsNullOrEmpty(stripeSignature))
                {
                    return BadRequest(new { error = "Missing Stripe-Signature header" });
                }

                var success = await _paymentService.HandlePaymentWebhook(json, stripeSignature);

                if (!success)
                {
                    return BadRequest(new { error = "Webhook processing failed" });
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("payment-status/{paymentIntentId}")]
        public async Task<IActionResult> CheckPaymentStatus(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                var status = paymentIntent.Status switch
                {
                    "requires_payment_method" => "Waiting for payment",
                    "requires_confirmation" => "Requires confirmation",
                    "requires_action" => "Requires customer action",
                    "processing" => "Processing",
                    "requires_capture" => "Requires capture",
                    "canceled" => "Canceled",
                    "succeeded" => "Paid",
                    _ => "Unknown status"
                };

                return Ok(new
                {
                    status,
                    paymentDate = paymentIntent.Status == "succeeded" ? DateTime.UtcNow : (DateTime?)null,
                    amountPaid = paymentIntent.AmountReceived / 100m,
                    currency = paymentIntent.Currency,
                    billId = paymentIntent.Metadata.TryGetValue("bill_id", out var billId) ? billId : null
                });
            }
            catch (StripeException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}