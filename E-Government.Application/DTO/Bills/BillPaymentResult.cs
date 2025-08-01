﻿namespace E_Government.Application.DTO.Bills
{
    public class BillPaymentResult
    {
        public bool Success { get; set; }
        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal Amount { get; set; }
        public string BillNumber { get; set; }
        public string ErrorMessage { get; set; }
    }
}