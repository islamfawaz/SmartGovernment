using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities
{
   public class MeterReading
    {
        public int Id { get; set; }
        public DateTime ReadingDate { get; set; } // تاريخ القراءة
        public decimal Value { get; set; } // قيمة القراءة
        public bool IsEstimated { get; set; } // قراءة حقيقية ولا تقديرية

        // علاقته بالعداد
        public int MeterId { get; set; }
        public Meter Meter { get; set; }
    }
}
