namespace E_Government.Core.Domain.Entities
{
    public enum BillStatus
    {
        Pending = 0,
        Paid = 1,
        Overdue = 2,
        Cancelled = 3,
        PaymentFailed = 4
    }
}