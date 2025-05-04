using E_Government.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.ServiceContracts
{
   public interface IBillingService
    {
        Task<BillPaymentResult> GenerateAndPayBill(GenerateBillRequestDto request);
        Task<MeterRegistrationResult> RegisterMeter(RegisterMeterDto request);

    }
}
