using E_Government.Application.DTO.Auth;
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

    }
}
