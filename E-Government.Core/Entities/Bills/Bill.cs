using E_Government.Domain.Entities;

namespace E_Government.Domain.Entities.Bills
{
    public class Bill
    {
        public int Id { get; set; }
        public string BillNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaymentDate { get; set; } // Nullable for unpaid bills
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public decimal UnitPrice { get; set; }
        public MeterType Type { get; set; }

        // Meter readings
        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; }

        // Relationships
        public int MeterId { get; set; }
        public Meter Meter { get; set; }

        public string UseNID { get; set; }
        public ApplicationUser User { get; set; }

        public string ? PdfUrl { get; set; }
        public string ? StripePaymentId { get; set; }
        public string ? PaymentId { get; set; } 


        // Calculated properties
        public decimal Consumption => CurrentReading - PreviousReading;
        public decimal TotalAmount => Consumption * UnitPrice;
    }
}