using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Infrastructure;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.ServiceContracts;
using E_Government.Core.Services;
using E_Government.Infrastructure.EGovernment_Unified;
using E_Government.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Government.Infrastructure
{
    public class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
             IServiceCollection services,
            IConfiguration configuration)
        {
            // تسجيل DbContext
            services.AddDbContext<UnifiedDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("EGovernment_Unified")));

            // تسجيل Identity
            services.AddIdentityCore<ApplicationUser>()
                .AddEntityFrameworkStores<UnifiedDbContext>();


            // تسجيل IUnitOfWork و الخدمات الأخرى
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // تسجيل خدمات أخرى
            services.AddScoped<SafeDeleteService>();
            services.AddScoped<IBillNumberGenerator, BillNumberGenerator>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDbInitializer, ApplicationDbInitializer>();

            return services;
        }
    }
}
