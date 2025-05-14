using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class UpdateRequestStatusInputDto
    {
        public string Status { get; set; } // e.g., "Approved", "Rejected"
        public string Notes { get; set; } // Reason for rejection or approval notes
    }
}
