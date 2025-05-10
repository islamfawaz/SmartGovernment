using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Government.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentService> _logger;
        private readonly StripeSettings _stripeSettings;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IOptions<StripeSettings> stripeSettings,
            ILogger<PaymentService> logger
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;

            if (stripeSettings?.Value == null)
            {
                _logger.LogError("Stripe settings are null!");
                throw new ArgumentNullException(nameof(stripeSettings));
            }
            _stripeSettings = stripeSettings.Value;
            if (string.IsNullOrEmpty(_stripeSettings.SecretKey))
            {
                _logger.LogError("Stripe Secret Key is null or empty!");
                throw new ArgumentException("Stripe Secret Key is required");
            }

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            _logger.LogInformation($"Stripe API Key initialized: {_stripeSettings.SecretKey?.Substring(0, 5)}...");
        }

        public async Task<BillPaymentResult> CreatePaymentIntent(BillPaymentRequest request)
        {
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            _logger.LogInformation("Creating payment intent for bill ID: {BillId}", request.BillId);

            var bill = await _unitOfWork.GetRepository<Bill, int>().GetAsync(request.BillId);

            if (bill is null)
                return new BillPaymentResult
                {
                    Success = false,
                    ErrorMessage = $"Bill with ID {request.BillId} not found"
                };

            if (bill.Status != BillStatus.Pending)
                return new BillPaymentResult
                {
                    Success = false,
                    ErrorMessage = "Bill is not in a payable state"
                };

            try
            {
                PaymentIntent paymentIntent;
                var paymentIntentService = new PaymentIntentService();

                // Create a new payment intent
                paymentIntent = await CreateNewPaymentIntent(bill);
                _logger.LogInformation("Created payment intent {PaymentIntentId} for bill {BillId}", paymentIntent.Id, bill.Id);

                bill.StripePaymentId = paymentIntent.Id;
                _unitOfWork.GetRepository<Bill, int>().Update(bill);
                await _unitOfWork.CompleteAsync();

                // Ensure paymentIntent is valid
                if (paymentIntent == null || string.IsNullOrEmpty(paymentIntent.Id) || string.IsNullOrEmpty(paymentIntent.ClientSecret))
                {
                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = "Failed to create payment intent"
                    };
                }

                return new BillPaymentResult
                {
                    Success = true,
                    PaymentIntentId = paymentIntent.Id,
                    ClientSecret = paymentIntent.ClientSecret,
                    Amount = (decimal)paymentIntent.Amount / 100,
                    BillNumber = bill.BillNumber
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error while creating payment intent for bill {BillId}: {Message}", request.BillId, ex.Message);
                return new BillPaymentResult
                {
                    Success = false,
                    ErrorMessage = $"Stripe error: {ex.Message}"
                };
            }
        }

        private async Task<PaymentIntent> CreateNewPaymentIntent(Bill bill)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(bill.Amount * 100),
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                Metadata = new Dictionary<string, string>
                {
                    { "bill_id", bill.Id.ToString() }
                }
            };

            return await new PaymentIntentService().CreateAsync(options);
        }

        public async Task<bool> HandlePaymentWebhook(string requestBody, string signatureHeader)
        {
            try
            {
                _logger.LogInformation("Received webhook with signature: {SignatureHeader}", signatureHeader?.Substring(0, 10) + "...");

                if (string.IsNullOrEmpty(_stripeSettings.WebhookSecret))
                {
                    _logger.LogError("Webhook secret is not configured");
                    return false;
                }

                var stripeEvent = EventUtility.ConstructEvent(
                    requestBody,
                    signatureHeader,
                    _stripeSettings.WebhookSecret,
                    throwOnApiVersionMismatch: false
                );

                _logger.LogInformation("Processing webhook event: {EventType} with ID: {EventId}", stripeEvent.Type, stripeEvent.Id);

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogInformation("Processing successful payment: {PaymentIntentId}", paymentIntent?.Id);
                        await HandleSuccessfulPayment(paymentIntent!);
                        break;

                    case "payment_intent.payment_failed":
                        var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                        _logger.LogWarning("Processing failed payment: {PaymentIntentId}", failedPayment?.Id);
                        await HandleFailedPayment(failedPayment!);
                        break;
                }

                return true;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe exception in webhook handler: {Message}", ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in webhook handler: {Message}", ex.Message);
                return false;
            }
        }

        private async Task HandleSuccessfulPayment(PaymentIntent paymentIntent)
        {
            try
            {
                _logger.LogInformation("Handling successful payment: {PaymentIntentId}", paymentIntent.Id);

                Bill bill = null;
                var billRepo = _unitOfWork.GetRepository<Bill, int>();

                // Use inline query instead of FindByStripePaymentIdAsync
                var bills = await billRepo.GetAllWithIncludeAsync(q => q.Where(b => b.StripePaymentId == paymentIntent.Id));
                bill = bills.FirstOrDefault();

                // If not found, try to find bill by metadata
                if (bill == null &&
                    paymentIntent.Metadata != null &&
                    paymentIntent.Metadata.TryGetValue("bill_id", out var billIdStr) &&
                    int.TryParse(billIdStr, out int billId))
                {
                    _logger.LogInformation("Bill not found by StripePaymentId, looking up by ID from metadata: {BillId}", billId);
                    bill = await billRepo.GetAsync(billId);

                    if (bill != null)
                    {
                        _logger.LogInformation("Found bill with ID {BillId} using metadata", billId);
                    }
                }

                if (bill == null)
                {
                    _logger.LogError("Could not find bill for payment intent {PaymentIntentId}", paymentIntent.Id);
                    return;
                }

                // Update bill status
                bill.Status = BillStatus.Paid;
                bill.PaymentDate = DateTime.UtcNow;
                bill.StripePaymentId = paymentIntent.Id;  // Ensure this is set even if found by ID

                billRepo.Update(bill);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Updated bill {BillId} to Paid status", bill.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling successful payment: {Message}", ex.Message);
                throw;
            }
        }

        private async Task HandleFailedPayment(PaymentIntent paymentIntent)
        {
            _logger.LogWarning("Handling failed payment: {PaymentIntentId}", paymentIntent.Id);

            Bill billToUpdate = null;
            var billRepo = _unitOfWork.GetRepository<Bill, int>();

            // Use inline query instead of FindByStripePaymentIdAsync
            var bills = await billRepo.GetAllWithIncludeAsync(q => q.Where(b => b.StripePaymentId == paymentIntent.Id));
            billToUpdate = bills.FirstOrDefault();

            // If not found, try to find by metadata
            if (billToUpdate == null &&
                paymentIntent.Metadata != null &&
                paymentIntent.Metadata.TryGetValue("bill_id", out var billIdStr) &&
                int.TryParse(billIdStr, out int billId))
            {
                _logger.LogInformation("Bill not found by StripePaymentId, looking up by ID from metadata: {BillId}", billId);
                billToUpdate = await billRepo.GetAsync(billId);

                if (billToUpdate != null)
                {
                    _logger.LogInformation("Found bill with ID {BillId} in failed payment metadata", billId);
                }
            }

            if (billToUpdate == null)
            {
                _logger.LogError("Bill not found for failed payment with intent {PaymentIntentId}", paymentIntent.Id);
                return;
            }

            billToUpdate.Status = BillStatus.PaymentFailed;
            billToUpdate.PaymentDate = null;

            billRepo.Update(billToUpdate);
            await _unitOfWork.CompleteAsync();
            _logger.LogWarning("Updated bill {BillId} to PaymentFailed status", billToUpdate.Id);
        }
    }
}