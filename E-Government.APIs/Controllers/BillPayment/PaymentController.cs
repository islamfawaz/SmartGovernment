using E_Government.APIs.Controllers.Base;
using E_Government.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using IPaymentService = E_Government.Core.ServiceContracts.IPaymentService;

namespace E_Government.UI.Controllers.BillsPayment
{
    /// <summary>
    /// Controller for handling bill payment operations.
    /// </summary>
    [Route("api/[controller]")]
    public class PaymentController : ApiControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class.
        /// </summary>
        /// <param name="paymentService">The payment service to handle payment operations.</param>
        /// <param name="logger">Logger for payment controller.</param>
        public PaymentController(
            IPaymentService paymentService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Initiates a bill payment.
        /// </summary>
        /// <param name="request">The bill payment request details.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
        [HttpPost("initiate-payment")]
        public async Task<IActionResult> InitiateBillPayment([FromBody] BillPaymentRequest request)
        {
            try
            {
                _logger.LogInformation("Initiating payment for bill ID: {BillId}", request.BillId);

                var result = await _paymentService.CreatePaymentIntent(request);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to create payment intent: {ErrorMessage}", result.ErrorMessage);
                    return BadRequest(new { error = result.ErrorMessage });
                }

                _logger.LogInformation("Successfully created payment intent: {PaymentIntentId}", result.PaymentIntentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating payment");
                return StatusCode(500, new { error = "An error occurred while processing your payment" });
            }
        }

        /// <summary>
        /// Handles Stripe payment webhooks.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the webhook processing.</returns>
        [HttpPost("webhook")]
        public async Task<IActionResult> HandlePaymentWebhook()
        {
            try
            {
                _logger.LogInformation("Received Stripe webhook");

                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();

                if (string.IsNullOrEmpty(stripeSignature))
                {
                    _logger.LogWarning("Missing Stripe-Signature header in webhook request");
                    return BadRequest(new { error = "Missing Stripe-Signature header" });
                }

                _logger.LogInformation("Processing webhook with signature: {SignaturePrefix}...",
                    stripeSignature.Substring(0, Math.Min(10, stripeSignature.Length)));

                var success = await _paymentService.HandlePaymentWebhook(json, stripeSignature);

                if (!success)
                {
                    _logger.LogWarning("Webhook processing failed");
                    return BadRequest(new { error = "Webhook processing failed" });
                }

                _logger.LogInformation("Webhook processed successfully");
                return Ok(new { received = true });
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe exception in webhook handler: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in webhook handler");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        /// <summary>
        /// Checks the status of a payment.
        /// </summary>
        /// <param name="paymentIntentId">The ID of the payment intent.</param>
        /// <returns>An <see cref="IActionResult"/> containing the payment status details.</returns>
        [HttpGet("payment-status/{paymentIntentId}")]
        public async Task<IActionResult> CheckPaymentStatus(string paymentIntentId)
        {
            try
            {
                _logger.LogInformation("Checking payment status for intent: {PaymentIntentId}", paymentIntentId);

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

                _logger.LogInformation("Payment intent {PaymentIntentId} status: {Status}", paymentIntentId, status);

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
                _logger.LogError(ex, "Stripe error checking payment status: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment status");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}