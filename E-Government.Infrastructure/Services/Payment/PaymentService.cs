using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Infrastructure;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.DTO;
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
            IOptions<StripeSettings> stripeSettings ,
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
            _logger.LogInformation("Stripe API Key initialized");


            var bill = await _unitOfWork.GetRepository<Bill>().GetAsync(request.BillId);

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

                // إنشاء نية دفع جديدة دائماً (بدلاً من محاولة إعادة استخدام القديمة)
                paymentIntent = await CreateNewPaymentIntent(bill);
                bill.StripePaymentId = paymentIntent.Id;

                _unitOfWork.GetRepository<Bill>().Update(bill);
                await _unitOfWork.CompleteAsync();

                // التأكد من أن paymentIntent ليست null وأنها تحتوي على القيم المطلوبة
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
                return new BillPaymentResult
                {
                    Success = false,
                    ErrorMessage = $"Stripe error: {ex.Message}"
                };
            }
        }
        private async Task<PaymentIntent> CreateNewPaymentIntent(Bill bill)
        {
            var paymentIntentService = new PaymentIntentService();

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(bill.Amount * 100),
                Currency = "usd",
                //PaymentMethodTypes = new List<string> { "card" },
             
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

 
                return await paymentIntentService.CreateAsync(options);
            
         
        }
        public async Task<bool> HandlePaymentWebhook(string requestBody, string signatureHeader)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    requestBody,
                    signatureHeader,
                    _stripeSettings.WebhookSecret
                );

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        await HandleSuccessfulPayment(paymentIntent!);
                        break;

                    case "payment_intent.payment_failed":
                        var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                        await HandleFailedPayment(failedPayment!);
                        break;

                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task HandleSuccessfulPayment(PaymentIntent paymentIntent)
        {
            if (!paymentIntent.Metadata.TryGetValue("bill_id", out var billIdStr) ||
                !int.TryParse(billIdStr, out var billId))
                return;

            var bill = await _unitOfWork.GetRepository<Bill>().GetAsync(billId);
            if (bill == null) return;

            bill.Status = BillStatus.Paid;
            bill.PaymentDate = DateTime.UtcNow; // Set payment date

            _unitOfWork.GetRepository<Bill>().Update(bill);
            await _unitOfWork.CompleteAsync();
        }

        private async Task HandleFailedPayment(PaymentIntent paymentIntent)
        {
            if (!paymentIntent.Metadata.TryGetValue("bill_id", out var billIdStr) ||
                !int.TryParse(billIdStr, out var billId))
                return;

            var bill = await _unitOfWork.GetRepository<Bill>().GetAsync(billId);
            if (bill == null) return;

            bill.Status = BillStatus.PaymentFailed;
            bill.PaymentDate = null; // Clear payment date if payment failed

            _unitOfWork.GetRepository<Bill>().Update(bill);
            await _unitOfWork.CompleteAsync();
        }
    }
}