// E-Government.APIs/Program.cs
using E_Government.Application; // Needed for scanning Application assembly
using E_Government.Core;

// using E_Government.Mediator; // Removed Mediator reference
using E_Government.Core.DTO; // Keep if StripeSettings is used directly here
using E_Government.Infrastructure;
using Microsoft.OpenApi.Models;
using Scrutor; // Added for Scrutor
using System.Reflection;

namespace E_Government.APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

            Console.WriteLine("API: Configuring services...");

            // Add services to the container.
            services.AddControllers();

            // Register Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Government API",
                    Version = "v1",
                    Description = "Documentation for the E-Government system"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            // Configure specific settings
            services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));

            // --- Register Infrastructure Services (including DbContext, UnitOfWork, Repositories) ---
            Console.WriteLine("API: Registering Infrastructure services...");
            // Call the extension method from the Infrastructure project directly
            // This handles DbContext, IUnitOfWork, IGenericRepository, and potentially others
            services.AddInfrastructureServices(configuration);
            Console.WriteLine("API: Infrastructure services registered.");

            // --- Use Scrutor to Scan and Register Application Services ---
            Console.WriteLine("API: Scanning Application assembly with Scrutor...");
            services.Scan(scan => scan
                // Scan the assembly containing Application services (e.g., ApplicationServiceExtensions)
                .FromAssemblyOf<PlaceholderApplicationClass>() // Use non-static placeholder
                    // Register classes implementing interfaces automatically
                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service"))) // Example: Register types ending in "Service"
                        .UsingRegistrationStrategy(RegistrationStrategy.Skip) // Avoid re-registering if already done
                        .AsMatchingInterface() // Match ClassName to IClassName
                        .WithScopedLifetime() // Or Transient/Singleton as needed
                    // Add more scanning rules if necessary
            );
            Console.WriteLine("API: Application assembly scanning complete.");

            // --- Removed Mediator call --- 
            // Console.WriteLine("API: Calling Mediator to register all layers...");
            // builder.Services.AddAllLayers(builder.Configuration);
            // Console.WriteLine("API: Mediator registration complete.");

            var app = builder.Build();

            Console.WriteLine("API: Configuring middleware pipeline...");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Government API v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            Console.WriteLine("API: Starting application...");
            app.Run();
        }
    }
}





