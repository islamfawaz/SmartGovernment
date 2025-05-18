using E_Government.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.ServiceContracts
{
    public interface IAuthService
    {
        Task<UserDTO> LoginAsync(loginDTO model);
        Task<UserDTO> RegisterAsync(RegisterDTO model);

        Task<UserDTO> GetCurrentUser(ClaimsPrincipal claimsPrincipal);



        Task<bool> EmailExist(string email);

    }
}
