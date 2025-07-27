using E_Government.Application.DTO.Bills;
using E_Government.Domain.DTO;

namespace E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure
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