using E_Government.Core.Domain.Entities;
using E_Government.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Government.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GenerateToken(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            // التحقق من المدخلات
            if (user == null)
            {
                _logger.LogError("User object is null");
                throw new ArgumentNullException(nameof(user));
            }

            if (userManager == null)
            {
                _logger.LogError("UserManager is null");
                throw new ArgumentNullException(nameof(userManager));
            }

            try
            {
                // إنشاء claims مع التحقق من القيم
                var claims = new List<Claim>
                {
                    //new Claim(ClaimTypes.NameIdentifier, user.Id ?? throw new ArgumentException("User Id is required")),
                    new Claim(ClaimTypes.Email, user.Email ?? "no-email@example.com"),
                    new Claim(ClaimTypes.GivenName, user.UserName ?? "unknown"),
                    new Claim("NID", user.NID ?? string.Empty)
                };

                // إضافة الأدوار
                var userRoles = await userManager.GetRolesAsync(user);
                claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

                // الحصول على إعدادات JWT
                var jwtKey = _configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
                var jwtIssuer = _configuration["JWT:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
                var jwtAudience = _configuration["JWT:Aud"] ?? throw new InvalidOperationException("JWT Audience is not configured");
                var jwtDuration = _configuration["JWT:Duration"] ?? "30"; // القيمة الافتراضية 30 يوم

                // توليد التوكن
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    expires: DateTime.Now.AddDays(double.Parse(jwtDuration)),
                    claims: claims,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token");
                throw; // إعادة رمي الاستثناء للطبقات الأعلى
            }
        }
    }
}