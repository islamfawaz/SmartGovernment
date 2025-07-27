namespace E_Government.Application.DTO.License
{
    public class StripePaymentDto
    {
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Amount { get; set; }
        public string BillNumber { get; set; }
        public string PaymentCode { get; set; }
    }
}
