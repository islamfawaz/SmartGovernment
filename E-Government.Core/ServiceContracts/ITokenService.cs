using E_Government.Core.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace E_Government.Core.ServiceContracts
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user, UserManager<ApplicationUser> userManager);
    }
}