// E-Government.Core/Helpers/StatusExtensions.cs
using E_Government.Core.Domain.Entities;
using E_Government.Core.DTO;

namespace E_Government.Core.Helpers
{
    public static class StatusExtensions
    {
        public static string ToDtoStatusString(this BillStatus status)
        {
            return status switch
            {
                BillStatus.Pending => "Pending",
                BillStatus.Paid => "Paid",
                BillStatus.PaymentFailed => "PaymentFailed",
                BillStatus.Canceled => "Canceled",
                BillStatus.Overdue => "Overdue",
                _ => "Pending"
            };
        }

        public static BillStatus ToEntityStatus(this string status)
        {
            return status switch
            {
                "Pending" => BillStatus.Pending,
                "Paid" => BillStatus.Paid,
                "PaymentFailed" => BillStatus.PaymentFailed,
                "Canceled" => BillStatus.Canceled,
                "Overdue" => BillStatus.Overdue,
                _ => BillStatus.Pending
            };
        }
    }
}