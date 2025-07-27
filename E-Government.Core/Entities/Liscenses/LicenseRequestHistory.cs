using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.Entities.Liscenses
{
    public class LicenseRequestHistory
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime ChangedAt { get; set; }
        public Guid RequestId { get; set; }
        public LicenseRequest Request { get; set; }


    }
}
