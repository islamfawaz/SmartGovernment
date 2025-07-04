using E_Government.Domain.Entities;
using E_Government.Domain.Entities.Liscenses;

namespace E_Government.Domain.Entities.Bills
{
    public class Bill
    {
        public int Id { get; set; }

        public string NID { get; set; }

        public string BillNumber { get; set; }

        public string ServiceCode { get; set; } // نوع الخدمة: DRIVING_RENEWAL, TRAFFIC_FINE ...

        public string? Description { get; set; }

        public DateTime ? IssueDate { get; set; }

        public DateTime ? DueDate { get; set; }

        public DateTime? PaymentDate { get; set; }

        public decimal Amount { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? PreviousReading { get; set; }

        public decimal? CurrentReading { get; set; }

        public MeterType? Type { get; set; }

        public BillStatus Status { get; set; }

        public int? MeterId { get; set; }

        public Meter? Meter { get; set; }

        public string UseNID { get; set; }

        public ApplicationUser? User { get; set; }

        public string? PdfUrl { get; set; }

        public string? StripePaymentId { get; set; }

        public string? PaymentId { get; set; }

        public Guid RequestId { get; set; }
        public LicenseRequest Request { get; set; }



        // ===== Calculated Properties (NotMapped) =====
        public decimal? Consumption =>
            (CurrentReading.HasValue && PreviousReading.HasValue)
                ? CurrentReading.Value - PreviousReading.Value
                : null;

        public decimal? TotalAmount =>
            (Consumption.HasValue && UnitPrice.HasValue)
                ? Consumption.Value * UnitPrice.Value
                : null;
    }
}