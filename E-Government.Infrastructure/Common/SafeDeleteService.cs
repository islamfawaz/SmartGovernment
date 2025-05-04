using E_Government.Infrastructure.EGovernment_Unified;

public class SafeDeleteService
{
    private readonly UnifiedDbContext _context;

    public SafeDeleteService(UnifiedDbContext context)
    {
        _context = context;
    }

    public async Task DeleteCustomerWithDependencies(int customerId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. حذف جميع المدفوعات المرتبطة
            //var payments =  _context.Payments
            //    .Where(p => p.UserNID == customerId)
            //    .ToList();
            //_context.Payments.RemoveRange(payments);

            // 2. حذف جميع العدادات المرتبطة
            //var meters =  _context.Meters
            //    .Where(m => m.UserNID == customerId)
            //    .ToList();
            //_context.Meters.RemoveRange(meters);

            // 3. حذف العميل نفسه
            

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}