using E_Government.Application.DTO.Bills;
using E_Government.Application.DTO.CivilDocs;
using E_Government.Application.DTO.License;
using E_Government.Domain.Entities.Liscenses;

namespace E_Government.Application.ServiceContracts
{
    public interface ILicenseService
    {
        Task<Guid> SubmitRequest(LicenseRequestDto requestDto);
        Task<LicenseRequest?> GetRequestByIdAsync(Guid id);
        Task<PaymentCodeDto> GeneratePaymentCode(LicenseRequest request);
        Task NotifyUserForPayment(LicenseRequest request, PaymentCodeDto paymentCode);

        Task<StripePaymentDto> InitiateStripePayment(string paymentCode);
        Task<bool> CompletePayment(string paymentIntentId);

        Task<bool> UpdateLicenseRequestStatusAsync(Guid id, string newStatus, string? notes, bool sendSignalRNotification = true);

        Task<PaymentCodeDto> ApproveLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input);
        Task<bool> RejectLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input);






    }
}
