using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.License
{
    public class PaymentCodeDto
    {
        public string PaymentCode { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string BillNumber { get; set; }
        public string ServiceDescription { get; set; }
    }
}
