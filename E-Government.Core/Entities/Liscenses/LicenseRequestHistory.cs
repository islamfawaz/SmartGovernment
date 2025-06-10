using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.Entities.Liscenses
{
   public class LicenseRequestHistory
    {

      public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LicensePublicId { get; set; } // Links to the PublicId of the license entity
        public int LicenseEntityIntId { get; set; } // Original Int ID of the license entity
        public string LicenseType { get; set; } // Type of license (e.g., "DrivingLicenseRenewal")
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
