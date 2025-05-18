// using E_Government.APIs.Extensions; // تأكد من وجود هذا إذا كان AddCoreServices منه
// using E_Government.Application.Services; // قد لا تحتاج هذا مباشرة هنا
using E_Government.APIs.Extensions;
using E_Government.Application.Services;
using E_Government.Application.Services.Admin; // لـ IAdminService و AdminServiceCorrected (أو اسم خدمتك المحدثة)
using E_Government.Application.Services.Auth;
using E_Government.Application.Services.License;
using E_Government.Application.Services.Prediction;
using E_Government.Core.Domain.Entities.Liscenses;
using E_Government.Core.Domain.RepositoryContracts.Persistence;

// using E_Government.Application.Services.License; // إذا كنت تسجل خدمات أخرى
// using E_Government.Core.Domain.RepositoryContracts.Persistence; // إذا كنت تسجل مستودعات هنا
using E_Government.Core.DTO;
using E_Government.Core.Helper.Hub;
using E_Government.Core.ServiceContracts;
using E_Government.Infrastructure;
using E_Government.Infrastructure.Common;
using E_Government.Infrastructure.Generic_Repository;

// using E_Government.Infrastructure.Services; // قد لا تحتاج هذا مباشرة هنا
using MapsterMapper;
// using Stripe.V2; // تأكد من أن هذا هو Stripe الصحيح، عادة ما يكون Stripe.net
using Microsoft.ML; // لـ StatusCodes

namespace E_Government.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

            Console.WriteLine("API: Configuring services...");

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://127.0.0.1:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddControllers();



            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
            services.AddIdentityService(configuration);
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IMapper, Mapper>(); // تأكد من أن هذا هو التنفيذ الصحيح لـ Mapster
            services.AddScoped<IBillingService, BillingServices>();
            services.AddScoped<ICivilDocumentsService, CivilDocumentsService>();
            services.AddScoped<ILicenseService, LicenseService>();
            services.AddScoped<DrivingLicenseRenewalRepository>();
            services.AddScoped<LicenseReplacementRequestRepository>();
            services.AddScoped<VehicleLicenseRenewalRepository>();
            services.AddScoped<IBillPredictionService, BillPredictionService>();

            services.AddScoped<IGenericRepository<DrivingLicenseRenewal, int>, GenericRepository<DrivingLicenseRenewal, int>>();
            services.AddScoped<IGenericRepository<LicenseReplacementRequest, int>, GenericRepository<LicenseReplacementRequest, int>>();
            services.AddScoped < IGenericRepository <VehicleRenwal, int>, GenericRepository<VehicleRenwal, int>>();
            builder.Services.AddSingleton<MLContext>();
            services.AddScoped<IPredictionService, PredictionService>();

            // Factory
            services.AddScoped<ILicenseRepositoryFactory, LicenseRepositoryFactory>();

            // Optional: generic repo for other entities
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            // ***** تعديل مهم: تسجيل خدمة المسؤولين الصحيحة *****
            // استبدل AdminService بالاسم الصحيح للخدمة التي تحتوي على تكامل SignalR
            // على سبيل المثال: AdminServiceCorrected أو AdminServiceWithSignalR
            services.AddScoped<IAdminService, AdminService>(); // <<<< غير هذا السطر
            services.AddScoped<IAuthService, AuthService>();
            // services.AddCoreServices(); // تأكد من أن هذه الدالة لا تقوم بتسجيل IAdminService مرة أخرى بشكل خاطئ
            services.AddSignalR();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddInfrastructureServices(configuration);

            var app = builder.Build();
            await app.InitializeDbAsync();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseRouting();

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseAuthentication(); // لازم يكون قبل UseAuthorization
            app.UseAuthorization();
            app.MapControllers();

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"IsAuthenticated: {context.User?.Identity?.IsAuthenticated}");
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    Console.WriteLine($"Claims: {string.Join(", ", context.User.Claims.Select(c => $"{c.Type}: {c.Value}"))}");
                }
                else
                {
                    Console.WriteLine($"Authentication Failed: {context.Response.StatusCode} - {context.Response.Headers["WWW-Authenticate"]}");
                }
                await next();
            });

            app.MapControllers();
            app.MapHub<DashboardHub>("/dashboardHub");

            app.UseSwagger();
            app.UseSwaggerUI();

            Console.WriteLine("API: Starting application...");
            app.Run();
        }
    }
}
