using E_Government.Application.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.ServiceContracts.Common
{
    public interface IServiceManager
    {
        public IBillingService BillingService { get;  }
        public ICivilDocumentsService CivilDocsService { get; }
        public INIDValidationService ValidationService { get;  }
        public IOTPService OTPService { get;  }
    }
}
