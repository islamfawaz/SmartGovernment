using E_Government.Application.DTO.OTP;
using E_Government.Domain.Entities.OTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts
{
    public interface IOTPService
    {
        Task<OtpResponseDTO> SendOtpAsync(string email, string purpose);
        Task InvalidateOtpAsync(string email, string purpose);
        Task<bool> CanSendOtpAsync(string email);
        Task CleanupExpiredOtpsAsync();
        Task<string> GenerateOtpCodeAsync();
        Task<bool> IsOtpValidAsync(string email, string code, string purpose);
        Task<OtpResponseDTO> VerifyOtpAsync(VerifyOtpDTO model);
    }
}
