using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure
{
    public interface IGovernorateService
    {
        public bool IsValidGovernorateCode(int code);
        public string GetGovernorateName(int code);

    }
}
