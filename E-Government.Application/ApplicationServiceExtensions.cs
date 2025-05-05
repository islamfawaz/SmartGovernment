// E-Government.Application/ApplicationServiceExtensions.cs
using Microsoft.Extensions.Configuration; // Added for consistency, might be needed
using Microsoft.Extensions.DependencyInjection;

namespace E_Government.Application
{
    public static class ApplicationServiceExtensions
    {
        // This method can be used for manual registrations specific to the Application layer,
        // although Scrutor will handle most convention-based registrations.
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            Console.WriteLine("Registering Application specific services (if any)...");

            // Add MediatR, AutoMapper, FluentValidation, etc., here if used.
            // Example:
            // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());
            // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Add any other specific Application service registrations here.

            Console.WriteLine("Application specific services registered.");
            return services;
        }
    }

    // Placeholder class for assembly scanning
    public class PlaceholderApplicationClass { }
}

