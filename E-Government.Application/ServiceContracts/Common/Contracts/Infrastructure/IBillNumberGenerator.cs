using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure
{
    public interface IBillNumberGenerator
    {
        Task<string> Generate();
    }
}
