using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
   public class StripeSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string PublishableKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
    }
}
