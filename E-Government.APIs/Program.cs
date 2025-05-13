using E_Government.Application.Services;
using E_Government.Application.Services.License;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using E_Government.Infrastructure;
using E_Government.Infrastructure.Services;
using Stripe.V2;

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

            // Add CORS Policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.SetIsOriginAllowed(origin => true) // This is more flexible than AllowAnyOrigin
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
            });

            // Add services to the container
            services.AddControllers();

            // Configure settings
            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));

            services.AddScoped<IBillingService, BillingServices>();
            services.AddScoped<ICivilDocumentsService, CivilDocumentsService>();
            services.AddScoped<ILicenseService,LicenseService>(); 
            // Register Core services
            services.AddCoreServices();

            // Add these lines in the service configuration section
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            
            // Register Infrastructure services through Core interfaces
            services.AddInfrastructureServices(configuration);

            var app = builder.Build();

            // Configure middleware
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseAuthorization();
            app.MapControllers();
            app.UseSwagger();
            app.UseSwaggerUI();

            Console.WriteLine("API: Starting application...");
            app.Run();
        }
    }
}

