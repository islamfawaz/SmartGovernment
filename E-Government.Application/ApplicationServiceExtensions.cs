// E-Government.Application/ApplicationServiceExtensions.cs
using Azure.Storage.Blobs;
using E_Government.Application.Helper;
using E_Government.Application.ServiceContracts;
using E_Government.Application.Services;
using E_Government.Application.Services.Admin;
using E_Government.Application.Services.Auth;
using E_Government.Application.Services.Common;
using E_Government.Application.Services.File;
using E_Government.Application.Services.License;
using E_Government.Application.Services.NIDValidation;
using E_Government.Application.Services.OTP;
using E_Government.Application.Services.Prediction;
using E_Government.Domain.Helper;
using E_Government.Domain.ServiceContracts.Common;
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
            services.AddScoped<IModelService, ModelService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ILicenseService, LicenseService>();
            services.AddSingleton<MLContext>();
            services.AddScoped<IPredictionService, PredictionService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INIDValidationService, NIDValidationService>();
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddTransient<IMailSettings, MailSettings>();
            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
           services.AddSingleton(x =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new BlobServiceClient(connectionString);
            });


            services.AddSignalR();
            return services;
        }
    }
}

