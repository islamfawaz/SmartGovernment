using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Infrastructure;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.ServiceContracts;
using E_Government.Infrastructure.EGovernment_Unified;
using E_Government.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Government.Infrastructure
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration) // This requires the parameter
        {
            // Register only UnifiedDbContext
            services.AddDbContext<UnifiedDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("EGovernment_Unified")));

            // Register Identity
            services.AddIdentity<ApplicationUser, IdentityRole<string>>()
                .AddEntityFrameworkStores<UnifiedDbContext>()
                .AddDefaultTokenProviders();

            // Register other services
            services.AddScoped<SafeDeleteService>();
            services.AddScoped<IBillNumberGenerator, BillNumberGenerator>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDbInitializer, ApplicationDbInitializer>();

            return services;

        }
    }
}