using E_Government.Core.DTO;

namespace E_Government.Core.ServiceContracts
{
    public interface IPaymentService
    {
        // Add payment service methods here
        //Task<bool> ProcessPaymentAsync(decimal amount, string currency);
        //Task<bool> ValidatePaymentAsync(string paymentId);
        Task<BillPaymentResult> CreatePaymentIntent(BillPaymentRequest request);
        Task<bool> HandlePaymentWebhook(string requestBody, string signatureHeader);
    }
} 