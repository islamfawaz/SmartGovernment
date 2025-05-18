using E_Government.Application.Services.Auth;
using E_Government.Core.Domain.Entities;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using E_Government.Infrastructure.EGovernment_Unified;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Government.APIs.Extensions
{
    public static class IdentityExtentions
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Identity and define password and lockout rules (optional, commented out)
            services.AddIdentity<ApplicationUser, IdentityRole>((identityOptions) =>
            {
                // identityOptions.Password.RequiredLength = 6;  // Example of enforcing password rules
            })
            .AddEntityFrameworkStores<UnifiedDbContext>();  // Add identity store for persistence
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]!)),
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role
                };
            });


            // Register the authentication service (AuthService) for dependency injection
            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            services.Configure<JwtSettings>(configuration.GetSection("JWTSettings"));  // Configure JWT settings

            services.AddScoped(typeof(Func<IAuthService>), (servicesProvider) =>
            {
                return () => servicesProvider.GetRequiredService<IAuthService>();
            });

            return services;
        }
    }

}
