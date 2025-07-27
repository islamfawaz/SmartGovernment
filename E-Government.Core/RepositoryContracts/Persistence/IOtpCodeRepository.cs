using E_Government.Domain.Entities.OTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.RepositoryContracts.Persistence
{
    public interface IOtpCodeRepository :IGenericRepository<OtpCode,Guid>
    {
      Task InvalidateOtpAsync(string email, string purpose);
       Task<bool> IsOtpValidAsync(string email,string code, string purpose);

       Task<OtpCode?> GetLastOtpAsync(string email);

        void DeleteExpiredOtps();

    }
}
