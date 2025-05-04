using E_Government.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.RepositoryContracts.Infrastructure
{
    public interface IPaymentService
    {
        Task<BillPaymentResult> CreatePaymentIntent(BillPaymentRequest request);
        Task<bool> HandlePaymentWebhook(string requestBody, string signatureHeader);
    }
}
