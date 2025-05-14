using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class RequestSummaryDto
    {
        public string RequestId { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantNID { get; set; } // Added based on review (common in user entities)
        public string RequestType { get; set; } // e.g., "Driving License Application", "Birth Certificate Request"
        public DateTime RequestDate { get; set; } // Aligned with typical entity field names
        public string Status { get; set; }
        public string DetailsApiEndpoint { get; set; } // For API call to get full details
    }
}
