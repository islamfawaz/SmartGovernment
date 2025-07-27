using E_Government.Domain.Entities.Bills;

namespace E_Government.Application.DTO.Bills
{
  public class BillDto
    {
        public int CustomerId { get; set; }
        public MeterType BillType { get; set; } // Electricity, Water, Gas
        public decimal CurrentReading { get; set; }
        public string PaymentMethodId { get; set; } // مثلاً: stripe payment method id
    }
}
