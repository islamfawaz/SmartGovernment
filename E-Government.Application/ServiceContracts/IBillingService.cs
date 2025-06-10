using E_Government.Application.DTO.Bills;
using E_Government.Domain.DTO;

namespace E_Government.Application.ServiceContracts
{
   public interface IBillingService
    {
        Task<BillPaymentResult> GenerateAndPayBill(GenerateBillRequestDto request);
        Task<MeterRegistrationResult> RegisterMeter(RegisterMeterDto request);

    }
}
