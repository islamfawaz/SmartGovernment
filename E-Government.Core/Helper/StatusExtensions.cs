using E_Government.Core.Domain.Entities;
using E_Government.Core.DTO;

namespace E_Government.Core.Helpers
{
    public static class StatusExtensions
    {
        // Convert enum to display-friendly string (for DTOs/UI)
        public static string ToDtoStatusString(this BillStatus status)
        {
            return status switch
            {
                BillStatus.Pending => "Pending",
                BillStatus.Paid => "Paid",
                BillStatus.PaymentFailed => "Payment Failed", // Added space for display
                BillStatus.Cancelled => "Canceled",
                BillStatus.Overdue => "Overdue",
                _ => "Unknown"
            };
        }

        // Convert string to enum (for API input)
        public static BillStatus ToEntityStatus(this string status)
        {
            return status.ToLower() switch
            {
                "pending" => BillStatus.Pending,
                "paid" => BillStatus.Paid,
                "paymentfailed" or "payment failed" => BillStatus.PaymentFailed,
                "canceled" => BillStatus.Cancelled,
                "overdue" => BillStatus.Overdue,
                _ => BillStatus.Pending // Default fallback
            };
        }

        // NEW: Convert integer to enum (for database values)
        public static BillStatus ToEntityStatus(this int statusValue)
        {
            return Enum.IsDefined(typeof(BillStatus), statusValue)
                ? (BillStatus)statusValue
                : BillStatus.Pending;
        }

        // NEW: Convert enum to integer (for database operations)
        public static int ToStatusValue(this BillStatus status)
        {
            return (int)status;
        }
    }
}