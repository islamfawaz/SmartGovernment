using E_Government.Application.ServiceContracts;
using E_Government.Application.Services.Auth;
using E_Government.Domain.Entities;
using E_Government.Domain.Helper;
using E_Government.Infrastructure.EGovernment_Unified;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            // ✅ Configure JwtSettings
            services.Configure<JwtSettings>(configuration.GetSection("JWTSettings"));

            // ✅ Add Entity Framework DbContext
            services.AddDbContext<UnifiedDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("EGovernment_Unified2")));

            // ✅ Register ASP.NET Core Identity Services
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<UnifiedDbContext>()
            .AddDefaultTokenProviders();

            // ✅ CRITICAL FIX: Clear JWT claim mappings
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            // ✅ Get JWT settings for validation
            var jwtSettings = configuration.GetSection("JWTSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            // ✅ Configure JWT Authentication with enhanced validation
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = false;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.FromMinutes(5), // Allow 5 minutes clock skew

                    // ✅ CRITICAL FIX: These settings are key for symmetric key validation
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateActor = false,
                    ValidateTokenReplay = false,

                    // ✅ IMPORTANT: Don't validate signature lifetime for development
                    
                    LifetimeValidator = (notBefore, expires, token, param) =>
                    {
                        return expires > DateTime.UtcNow;
                    },

                    // Use standard JWT claim names
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Token;
                        if (!string.IsNullOrEmpty(token))
                        {
                            Console.WriteLine($"🔍 Token received: {token.Substring(0, Math.Min(50, token.Length))}...");
                        }
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("✅ Token validation successful!");
                        Console.WriteLine($"IsAuthenticated: {context.Principal.Identity.IsAuthenticated}");
                        Console.WriteLine($"Claims: {string.Join(", ", context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
                        return Task.CompletedTask;
                    },

                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"❌ Auth Failed: {context.Exception?.Message}");
                        Console.WriteLine($"❌ Exception Type: {context.Exception?.GetType().FullName}");
                        Console.WriteLine($"❌ Inner Exception: {context.Exception?.InnerException?.Message}");

                        // Detailed token analysis
                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                        if (authHeader?.StartsWith("Bearer ") == true)
                        {
                            var token = authHeader.Substring(7);
                            try
                            {
                                var handler = new JwtSecurityTokenHandler();
                                var jsonToken = handler.ReadJwtToken(token);
                                Console.WriteLine($"🔍 Token Header: {string.Join(", ", jsonToken.Header.Select(h => $"{h.Key}={h.Value}"))}");
                                Console.WriteLine($"🔍 Token Issuer: {jsonToken.Issuer}");
                                Console.WriteLine($"🔍 Token Audience: {string.Join(", ", jsonToken.Audiences)}");
                                Console.WriteLine($"🔍 Token Valid From: {jsonToken.ValidFrom}");
                                Console.WriteLine($"🔍 Token Valid To: {jsonToken.ValidTo}");
                                Console.WriteLine($"🔍 Current Time: {DateTime.UtcNow}");

                                // Check signature manually
                                var validationParams = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = jwtSettings["Issuer"],
                                    ValidAudience = jwtSettings["Audience"],
                                    IssuerSigningKey = new SymmetricSecurityKey(key),
                                    ClockSkew = TimeSpan.FromMinutes(5)
                                };

                                var principal = handler.ValidateToken(token, validationParams, out var validatedToken);
                                Console.WriteLine($"✅ Manual validation successful: {principal.Identity.IsAuthenticated}");
                            }
                            catch (Exception validationEx)
                            {
                                Console.WriteLine($"❌ Manual validation failed: {validationEx.Message}");
                            }
                        }

                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        Console.WriteLine($"🚫 JWT Challenge: {context.Error} - {context.ErrorDescription}");
                        Console.WriteLine($"🚫 Challenge AuthenticateFailure: {context.AuthenticateFailure?.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

            // ✅ Register custom services
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }    // ✅ Package Version Checker - Add this to help diagnose version issues
    public static class PackageVersionChecker
    {
        public static void LogPackageVersions()
        {
            try
            {
                var jwtAssembly = typeof(JwtSecurityToken).Assembly;
                var tokenAssembly = typeof(SecurityToken).Assembly;
                var identityModelAssembly = typeof(Base64UrlEncoder).Assembly;

                Console.WriteLine("=== Package Version Information ===");
                Console.WriteLine($"System.IdentityModel.Tokens.Jwt: {jwtAssembly.GetName().Version}");
                Console.WriteLine($"Microsoft.IdentityModel.Tokens: {tokenAssembly.GetName().Version}");
                Console.WriteLine($"Microsoft.IdentityModel.Abstractions: {identityModelAssembly.GetName().Version}");

                // Test Base64UrlEncoder availability
                try
                {
                    var testBytes = Base64UrlEncoder.DecodeBytes("dGVzdA");
                    Console.WriteLine("✅ Base64UrlEncoder working correctly");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Base64UrlEncoder issue: {ex.Message}");
                }

                Console.WriteLine("=====================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking package versions: {ex.Message}");
            }
        }
    }
}