// E-Government.Application/ApplicationServiceExtensions.cs
using E_Government.Application.ServiceContracts;
using E_Government.Application.Services;
using E_Government.Application.Services.Admin;
using E_Government.Application.Services.Auth;
using E_Government.Application.Services.License;
using E_Government.Application.Services.Prediction;
using E_Government.Domain.Helper;
using MapsterMapper;
using Microsoft.Extensions.Configuration; // Added for consistency, might be needed
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ML;

namespace E_Government.Application
{
    public static class ApplicationServiceExtensions
    {
       public static IServiceCollection AddApplicationServices(this IServiceCollection services ,IConfiguration configuration)
        {
            services.AddScoped<IMapper, Mapper>();
            services.AddScoped<IBillingService, BillingServices>();
            services.AddScoped<ICivilDocumentsService, CivilDocumentsService>();
            services.AddScoped<ILicenseService, LicenseService>();
            services.AddScoped<IModelService, ModelService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddSingleton<MLContext>();
            services.AddScoped<IPredictionService, PredictionService>();
            services.AddScoped<IAuthService, AuthService>();
            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
            

            services.AddSignalR();
            return services;
        }
    }
}

