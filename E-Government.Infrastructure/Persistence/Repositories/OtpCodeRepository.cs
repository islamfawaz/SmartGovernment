using E_Government.Domain.Entities.OTP;
using E_Government.Domain.RepositoryContracts.Persistence;
using E_Government.Infrastructure.Persistence._Data;
using E_Government.Infrastructure.Persistence.Generic_Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.Persistence.Repositories
{
    public class OtpCodeRepository : GenericRepository<OtpCode, Guid>, IOtpCodeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OtpCodeRepository(ApplicationDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public  void DeleteExpiredOtps()
        {
            var expireOtps =  _dbContext.OtpCodes.Where(o => o.ExpiresAt < DateTime.UtcNow);

            _dbContext.OtpCodes.RemoveRange(expireOtps!);
         }

        public async Task<OtpCode?> GetLastOtpAsync(string email)
        {
            var otp = await _dbContext.OtpCodes.Where(o => o.Email == email)
                .OrderByDescending(o=>o.CreatedAt)
                .FirstOrDefaultAsync();
            return otp;
        }

        public async Task InvalidateOtpAsync(string email, string purpose)
        {
            var existingOtp=await _dbContext.OtpCodes.Where(o=>o.Email==email && o.Purpose==purpose && !o.IsUsed).ToListAsync();

             foreach (var code in existingOtp)
            {
                code.IsUsed = true;
            }


        }

        public async Task<bool> IsOtpValidAsync(string email, string code, string purpose)
        {
            var otpCode= await _dbContext.OtpCodes.FirstOrDefaultAsync(o=>
            o.Email == email &&
            o.Purpose == purpose &&
            o.Code == code &&
            !o.IsUsed &&
            o.ExpiresAt > DateTime.UtcNow);

            return otpCode!=null;
        }

        
    }
}
