using E_Government.Application.Services;
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
            var reactAppOrigin = "http://localhost:5174";
            var corsPolicyName = "AllowReactApp";
            services.AddCors(options =>
            {
                options.AddPolicy(name: corsPolicyName,
                                  policy =>
                                  {
                                      policy.WithOrigins(reactAppOrigin)
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });

            // Add services to the container
            services.AddControllers();

            // Configure settings
            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));

            services.AddScoped<IBillingService, BillingServices>();
            services.AddScoped<CivilDocumentsService>();
            // Register Core services
            services.AddCoreServices();

            // Add these lines in the service configuration section
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add these lines in the middleware section (before app.Run())
            
            // Register Infrastructure services through Core interfaces
            services.AddInfrastructureServices(configuration);

            var app = builder.Build();

            // Configure middleware
            app.UseHttpsRedirection();
            app.UseCors(corsPolicyName);
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

