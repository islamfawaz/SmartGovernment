using E_Government.Core.DTO;

namespace E_Government.Core.ServiceContracts
{
   public interface IBillingService
    {
        Task<BillPaymentResult> GenerateAndPayBill(GenerateBillRequestDto request);
        Task<MeterRegistrationResult> RegisterMeter(RegisterMeterDto request);

    }
}
