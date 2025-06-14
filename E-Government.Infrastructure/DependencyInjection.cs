using E_Government.Application.DTO.License;
using E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure;
using E_Government.Domain.Entities.Liscenses;
using E_Government.Domain.RepositoryContracts.Persistence;
using E_Government.Infrastructure.Infrastructure.Services;
using E_Government.Infrastructure.Infrastructure.Services.User;
using E_Government.Infrastructure.Persistence._Data;
using E_Government.Infrastructure.Persistence.Generic_Repository;
using E_Government.Infrastructure.Persistence.Repositories;
using E_Government.Infrastructure.Persistence.UnitOfWork;
using E_Government.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Government.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // --- Register DbContext ---
            var connectionString = configuration.GetConnectionString("EGovernment_Unified2");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'EGovernment_Unified' not found...");
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));
            }


            services.AddScoped<IApplicationDbInitializer, ApplicationDbInitializer>();
            // --- Register UnitOfWork and Generic Repository ---
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            // --- Register Specific Infrastructure Services ---
            services.AddSingleton<IGovernorateService, GovernorateService>();

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBillNumberGenerator, BillNumberGenerator>();  
            services.AddScoped<DrivingLicenseRenewalRepository>();
            services.AddScoped<LicenseReplacementRequestRepository>();
            services.AddScoped<VehicleLicenseRenewalRepository>();
            services.AddScoped<IGenericRepository<DrivingLicenseRenewal, int>, GenericRepository<DrivingLicenseRenewal, int>>();
            services.AddScoped<IGenericRepository<LicenseReplacementRequest, int>, GenericRepository<LicenseReplacementRequest, int>>();
            services.AddScoped<IGenericRepository<VehicleRenwal, int>, GenericRepository<VehicleRenwal, int>>();
            services.AddScoped<ILicenseRepositoryFactory, LicenseRepositoryFactory>();
            services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();

            return services;
        }

    }
}

