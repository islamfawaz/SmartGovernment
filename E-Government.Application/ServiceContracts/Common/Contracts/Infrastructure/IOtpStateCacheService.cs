using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure
{
    public interface IOtpStateCacheService
    {
         Task SetVerifiedAsync(string email, string purpose);

         Task<bool> IsVerifiedAsync(string email, string purpose);


    }
}
