using E_Government.APIs.Controllers.Base;
using E_Government.Application.DTO.License;
using E_Government.Application.Exceptions;
using E_Government.Application.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Government.APIs.Controllers.License
{

    namespace E_Government.API.Controllers
    {
         
        public class LicenseController : ApiControllerBase
        {
            private readonly ILicenseService _licenseService;
            private readonly ILogger<LicenseController> _logger;

            public LicenseController(ILicenseService licenseService, ILogger<LicenseController> logger)
            {
                _licenseService = licenseService ?? throw new ArgumentNullException(nameof(licenseService));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            /// <summary>
            /// Submit a new license request
            /// </summary>
            [HttpPost("submit-request")]
            public async Task<IActionResult> SubmitRequest([FromBody] LicenseRequestDto dto)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    var requestId = await _licenseService.SubmitRequest(dto);
                    _logger.LogInformation("License request submitted successfully with ID: {RequestId}", requestId);

                    return Ok(new { RequestId = requestId, Message = "License request submitted successfully" });
                }
                catch (ArgumentException ex)
                {
                    _logger.LogWarning("Invalid argument in license request: {Message}", ex.Message);
                    return BadRequest(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error submitting license request");
                    return StatusCode(500, new { Error = "An error occurred while submitting the request" });
                }
            }

            /// <summary>
            /// Generate payment code for a license request
            /// </summary>
            [HttpPost("{requestId}/generate-payment-code")]
            public async Task<IActionResult> GeneratePaymentCode(Guid requestId)
            {
                try
                {
                    var request = await _licenseService.GetRequestByIdAsync(requestId);
                    if (request == null)
                    {
                        return NotFound(new { Error = "License request not found" });
                    }

                    var paymentCode = await _licenseService.GeneratePaymentCode(request);
                    _logger.LogInformation("Payment code generated for request {RequestId}", requestId);

                    return Ok(paymentCode);
                }
                catch (NotFoundException ex)
                {
                    _logger.LogWarning("Request not found: {Message}", ex.Message);
                    return NotFound(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating payment code for request {RequestId}", requestId);
                    return StatusCode(500, new { Error = "An error occurred while generating payment code" });
                }
            }

            /// <summary>
            /// Initiate Stripe payment for a license request
            /// </summary>
            [HttpPost("initiate-payment")]
            public async Task<IActionResult> InitiateStripePayment([FromBody] InitiatePaymentDto request)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(request.PaymentCode))
                    {
                        return BadRequest(new { Error = "Payment code is required" });
                    }

                    var paymentResult = await _licenseService.InitiateStripePayment(request.PaymentCode);
                    _logger.LogInformation("Stripe payment initiated for payment code: {PaymentCode}", request.PaymentCode);

                    return Ok(paymentResult);
                }
                catch (NotFoundException ex)
                {
                    _logger.LogWarning("Payment code not found: {Message}", ex.Message);
                    return NotFound(new { Error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning("Invalid payment operation: {Message}", ex.Message);
                    return BadRequest(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error initiating Stripe payment");
                    return StatusCode(500, new { Error = "An error occurred while initiating payment" });
                }
            }

            /// <summary>
            /// Complete payment for a license request
            /// </summary>
            [HttpPost("complete-payment")]
            public async Task<IActionResult> CompletePayment([FromBody] ConfirmPaymentDto request)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(request.PaymentIntentId))
                    {
                        return BadRequest(new { Error = "Payment intent ID is required" });
                    }

                    var result = await _licenseService.CompletePayment(request.PaymentIntentId);
                    _logger.LogInformation("Payment completed for payment intent: {PaymentIntentId}", request.PaymentIntentId);

                    return Ok(new { Success = result, Message = "Payment completed successfully" });
                }
                catch (NotFoundException ex)
                {
                    _logger.LogWarning("Payment intent not found: {Message}", ex.Message);
                    return NotFound(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error completing payment");
                    return StatusCode(500, new { Error = "An error occurred while completing payment" });
                }
            }

            /// <summary>
            /// Get license request details by ID
            /// </summary>
            [HttpGet("{requestId}")]
            public async Task<IActionResult> GetRequestById(Guid requestId)
            {
                try
                {
                    var request = await _licenseService.GetRequestByIdAsync(requestId);
                    if (request == null)
                    {
                        return NotFound(new { Error = "License request not found" });
                    }

                    return Ok(request);
                }
                catch (NotFoundException ex)
                {
                    _logger.LogWarning("Request not found: {Message}", ex.Message);
                    return NotFound(new { Error = ex.Message });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving license request {RequestId}", requestId);
                    return StatusCode(500, new { Error = "An error occurred while retrieving the request" });
                }
            }

        }
    }
}