using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class RequestHistoryDto // Generic history entry
    {
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string ChangedBy { get; set; } // User ID or name
    }
}
