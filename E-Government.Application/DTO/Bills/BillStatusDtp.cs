using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.Bills
{
    public enum BillStatusDtp
    {
        Pending,
        Paid,
        PaymentFailed,
        Canceled,
        Overdue
    }
}
