using E_Government.Application.DTO.Auth;
using E_Government.Application.DTO.OTP;
using E_Government.Application.DTO.User;
using E_Government.Domain.DTO;
using System.Security.Claims;

namespace E_Government.Application.ServiceContracts
{
    public interface IAuthService
    {
        Task<ApplicationUserDto> LoginAsync(loginDTO model);
        Task<ApplicationUserDto> RegisterAsync(RegisterDTO model);
        Task<ApplicationUserDto> GetCurrentUser(ClaimsPrincipal claimsPrincipal);
        Task<bool> EmailExist(string email);
        Task<OtpResponseDTO> SendRegistrationOtpAsync(string email);
        Task<ApplicationUserDto> RegisterWithOtpAsync(RegisterDTO model, string otpCode);
        Task<OtpResponseDTO> SendPasswordResetOtpAsync(string email);
        Task<bool> ResetPasswordWithOtpAsync(string email, string otpCode, string newPassword);

    }
}
