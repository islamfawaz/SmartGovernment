// using E_Government.APIs.Extensions; // تأكد من وجود هذا إذا كان AddCoreServices منه
// using E_Government.Application.Services; // قد لا تحتاج هذا مباشرة هنا
using E_Government.APIs.Extensions;
using E_Government.Application.Services;
using E_Government.Application.Services.Admin; // لـ IAdminService و AdminServiceCorrected (أو اسم خدمتك المحدثة)
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
using Microsoft.AspNetCore.Authentication.Cookies; // مثال لمصادقة الكوكيز
using Microsoft.AspNetCore.Http;
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

            // مثال: تكوين المصادقة (افترض أنك تستخدم مصادقة الكوكيز)
            // إذا كنت تستخدم JWT أو نظام آخر، ستحتاج إلى تعديل هذا الجزء
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // أو JwtBearerDefaults.AuthenticationScheme
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // مثال
                    options.SlidingExpiration = true;
                    options.Events.OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        }
                        else
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        }
                        else
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }
                        return Task.CompletedTask;
                    };
                });
            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
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

            // ***** تعديل مهم: إضافة UseAuthentication *****
            app.UseAuthentication(); // <<<< أضف هذا السطر هنا
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<DashboardHub>("/dashboardHub");

            app.UseSwagger();
            app.UseSwaggerUI();

            Console.WriteLine("API: Starting application...");
            app.Run();
        }
    }
}
