using E_Government.Application.DTO.Auth;
using E_Government.Application.DTO.User;
using E_Government.Application.Exceptions;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.DTO;
using E_Government.Domain.Entities;
using E_Government.Domain.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Government.Application.Services.Auth
{
    public class AuthService(
         UserManager<ApplicationUser> userManager,
         SignInManager<ApplicationUser> signInManager,
         IOptions<JwtSettings> jwtSettings,
         INIDValidationService validationService
        ) : IAuthService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;
        private readonly INIDValidationService _validationService = validationService;

        public async Task<bool> EmailExist(string email)
        {
            return await userManager.FindByEmailAsync(email!) is not null;
        }

        // Add this method to your AuthService class
        private async Task<ApplicationUser> CreateUserFromTokenClaimsAsync(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirst("uid")?.Value;
            var email = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            var displayName = claimsPrincipal.FindFirst("display_name")?.Value;
            var nid = claimsPrincipal.FindFirst("nid")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                throw new BadRequestException("Cannot create user: email not found in token");
            }

            Console.WriteLine($"🔧 Auto-creating user from token claims: {email}");

            var user = new ApplicationUser
            {

                Id = userId, // Use the same ID from the token
                Email = email,
                UserName = email,
                DisplayName = displayName ?? email,
                NID = nid?.Trim(),
                EmailConfirmed = true // Since they had a valid token
            };

            // Create user without password (since they're already authenticated via token)
            var result = await userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine($"❌ Failed to create user: {errors}");
                throw new BadRequestException($"Failed to recreate user: {errors}");
            }

            Console.WriteLine($"✅ Successfully created user: {user.Email}");
            return user;
        }

        // Then modify your GetCurrentUser method to use this:
        public async Task<ApplicationUserDto> GetCurrentUser(ClaimsPrincipal claimsPrincipal)
        {
            if (!claimsPrincipal.Identity.IsAuthenticated)
                throw new UnAuthorizedException("User is not authenticated.");

            var userId = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ??
                        claimsPrincipal.FindFirst("uid")?.Value;

            var email = claimsPrincipal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;

            Console.WriteLine($"🔍 Looking for user - ID: '{userId}', Email: '{email}'");

            ApplicationUser user = null;

            // Try to find user by ID
            if (!string.IsNullOrEmpty(userId))
            {
                user = await userManager.FindByIdAsync(userId);
            }

            // Try to find user by email
            if (user == null && !string.IsNullOrEmpty(email))
            {
                user = await userManager.FindByEmailAsync(email);
            }

            // If user still not found, auto-create from token claims
            if (user == null)
            {
                Console.WriteLine("⚠️ User not found in database, attempting to recreate from token...");
                try
                {
                    user = await CreateUserFromTokenClaimsAsync(claimsPrincipal);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Failed to recreate user: {ex.Message}");
                    throw new NotFoundException($"User not found and could not be recreated: {ex.Message}");
                }
            }

            Console.WriteLine($"✅ Returning user: {user.Email}");

            return new ApplicationUserDto
            {
                DisplayName = user.DisplayName,
                NID = user.NID,
                Email = user.Email!,
                Token = await GenerateTokenAsync(user)
            };
        }

        public async Task<ApplicationUserDto> LoginAsync(loginDTO model)
        {
            var normalizedEmail = model.Email.ToLower();
            var user = await userManager.FindByEmailAsync(normalizedEmail);

            if (user is null) throw new UnAuthorizedException("Invalid Login attempt");

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);
            if (result.IsNotAllowed) throw new UnAuthorizedException("Account not Confirmed Yet.");
            if (result.IsLockedOut) throw new UnAuthorizedException("Account Is Locked.");
            if (!result.Succeeded) throw new UnAuthorizedException("Invalid Login attempt");

            return new ApplicationUserDto()
            {
                DisplayName = user.DisplayName!,
                NID = user.NID,
                Email = user.Email!,
                Token = await GenerateTokenAsync(user)
            };
        }

        public async Task<ApplicationUserDto> RegisterAsync(RegisterDTO model)
        {
            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                throw new BadRequestException("This email already exists.");

            var user = new ApplicationUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                NID = model.NID,
                Address = model.Address,
                Category = model.Category,
                UserName = model.Email,
                Gender = _validationService.ExtractGender(model.NID),
                DateOfBirth=_validationService.ExtractDateOfBirth(model.NID),
                GovernorateName=_validationService.ExtractGovernorateInfo(model.NID).Name,
                


            };

            var result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"User creation failed: {errorMessages}");
            }

            return new ApplicationUserDto()
            {
                DisplayName = user.DisplayName,
                NID = user.NID,
                Email = user.Email,
                Token = await GenerateTokenAsync(user)
            };
        }




        private async Task<string> GenerateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
    {
        // ✅ FIXED: Use consistent claim types
        new Claim(ClaimTypes.NameIdentifier, user.Id), // This maps to nameidentifier
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Email, user.Email), // This maps to emailaddress
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Iat,
            new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64),
        new Claim("uid", user.Id), // Custom claim for backup
        new Claim("display_name", user.DisplayName ?? ""),
        new Claim("nid", user.NID ?? "")
    };

            var userClaims = await userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            Console.WriteLine($"✅ Token created successfully");
            Console.WriteLine($"✅ Token length: {tokenString.Length}");
            Console.WriteLine($"✅ Expires: {tokenDescriptor.Expires}");

            // ✅ DEBUG: Show what claims are in the token
            try
            {
                var decodedToken = tokenHandler.ReadJwtToken(tokenString);
                Console.WriteLine($"✅ Token validation: Header={decodedToken.Header.Count}, Claims={decodedToken.Claims.Count()}");
                Console.WriteLine("✅ Token Claims:");
                foreach (var claim in decodedToken.Claims)
                {
                    Console.WriteLine($"   {claim.Type} = {claim.Value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Token decode error: {ex.Message}");
            }

            return tokenString;
        }
    }
}
