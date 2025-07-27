using E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure;
using E_Government.Domain.Entities.Bills;
using E_Government.Domain.RepositoryContracts.Persistence;
using Microsoft.Extensions.Logging;

namespace E_Government.Infrastructure.Infrastructure.Services
{
    public class BillNumberGenerator : IBillNumberGenerator
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BillNumberGenerator> _logger;

        public BillNumberGenerator(
            IUnitOfWork unitOfWork,
            ILogger<BillNumberGenerator> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<string> Generate()
        {
            try
            {
                var billRepo = _unitOfWork.GetRepository<Bill, int>();
                var lastBill = (await billRepo.GetAllWithIncludeAsync(q => q.OrderByDescending(b => b.Id))).FirstOrDefault();

                int nextNumber = (lastBill?.Id ?? 0) + 1;
                string billNumber = $"BL-{DateTime.Now:yyyyMMdd}-{nextNumber:00000}";

                _logger.LogInformation("تم إنشاء رقم فاتورة: {BillNumber}", billNumber);
                return billNumber;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل في إنشاء رقم الفاتورة");
                return $"BL-{DateTime.Now:yyyyMMdd}-ERR-{DateTime.Now.Ticks}";
            }
        }
    }
}
