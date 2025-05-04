namespace E_Government.Core.DTO
{
    public class BillPaymentResult
    {
        public bool Success { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Amount { get; set; }
        public string BillNumber { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? PaymentDate { get; set; } // Optional payment date
        public string Status { get; set; } // Payment status
    }
}