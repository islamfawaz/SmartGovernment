using E_Government.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
  public class BillDto
    {
        public int CustomerId { get; set; }
        public MeterType BillType { get; set; } // Electricity, Water, Gas
        public decimal CurrentReading { get; set; }
        public string PaymentMethodId { get; set; } // مثلاً: stripe payment method id
    }
}
