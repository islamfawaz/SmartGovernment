namespace E_Government.Domain.Entities.Bills
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