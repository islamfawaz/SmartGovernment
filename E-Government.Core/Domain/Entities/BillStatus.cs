namespace E_Government.Core.Domain.Entities
{
    public enum BillStatus
    {
        Pending,        // معلقة
        Paid,           // مدفوعة
        PaymentFailed,  // فشل الدفع
        Canceled,       // ملغية
        Overdue         // متأخرة
    }
}