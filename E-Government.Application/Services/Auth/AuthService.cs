using E_Government.Core.Domain.Entities;
using E_Government.Core.DTO;
using E_Government.Core.Exceptions;
using E_Government.Core.ServiceContracts;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.Services.Auth
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtSettings,
        IMapper mapper) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public async Task<bool> EmailExist(string email)
        {
            return await userManager.FindByEmailAsync(email!) is not null;
        }

        public async Task<UserDTO> GetCurrentUser(ClaimsPrincipal claimsPrincipal)
        {
            if (!claimsPrincipal.Identity.IsAuthenticated)
                throw new UnAuthorizedException("User is not authenticated.");

            var email = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
                        ?? claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value
                        ?? claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        return new UserDTO
                        {
                            DisplayName = user.DisplayName,
                            NID = user.NID,
                            Email = user.Email!,
                            Token = await GenerateTokenAsync(user)
                        };
                    }
                }
                throw new UnAuthorizedException("User identification not found in token.");
            }

            var userByEmail = await userManager.FindByEmailAsync(email);
            if (userByEmail is null)
                throw new NotFoundException("User not found.");

            return new UserDTO
            {
                DisplayName = userByEmail.DisplayName,
                NID = userByEmail.NID,
                Email = userByEmail.Email!,
                Token = await GenerateTokenAsync(userByEmail)
            };
        }

        public async Task<UserDTO> LoginAsync(loginDTO model)
        {
            var normalizedEmail = model.Email.ToLower();
            var user = await userManager.FindByEmailAsync(normalizedEmail);

            if (user is null) throw new UnAuthorizedException("Invalid Login attempt");

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
            if (result.IsNotAllowed) throw new UnAuthorizedException("Account not Confirmed Yet.");
            if (result.IsLockedOut) throw new UnAuthorizedException("Account Is Locked.");
            if (!result.Succeeded) throw new UnAuthorizedException("Invalid Login attempt");

            return new UserDTO()
            {
                Email = user.Email,
                Token = await GenerateTokenAsync(user)
            };
        }

        public async Task<UserDTO> RegisterAsync(RegisterDTO model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                throw new BadRequestException("This email already exists.");

            var user = new ApplicationUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                NID = model.NID, // ❗ Primary Key
                Address = model.Address,
                Category = model.Category,
                UserName = model.Email // ✅ Required by Identity
            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"User creation failed: {errorMessages}");
            }

            return new UserDTO()
            {
                Email = user.Email,
                Token = await GenerateTokenAsync(user)
            };
        }

        private async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            // في دالة GenerateToken في TokenService
            var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id),
    new Claim(ClaimTypes.Email, user.Email), // استخدم ClaimTypes.Email بدلاً من "email"
    new Claim(ClaimTypes.GivenName, user.DisplayName)
    // أي claims أخرى تحتاجها
};


            // باقي الكود كما هو
            var userClaims = await userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var token = new JwtSecurityToken(
                audience: _jwtSettings.Audience,
                issuer: _jwtSettings.Issuer,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                claims: claims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
