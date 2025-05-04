using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
   public class BillPaymentRequest
    {
        public int BillId { get; set; }
        public string UserEmail { get; set; }

        public decimal Amount { get; set; }


    }
}
