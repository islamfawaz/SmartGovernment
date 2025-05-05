// E-Government.Mediator/ServiceCollectionExtensions.cs
using E_Government.Application; // Assuming AddApplicationService is here
using E_Government.Infrastructure; // Add direct reference to Infrastructure
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace E_Government.Mediator
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers services from Application and Infrastructure layers.
        /// This acts as the Composition Root for dependency injection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The IServiceCollection so that additional calls can be chained.</returns>
        public static IServiceCollection AddAllLayers(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            Console.WriteLine("Mediator: Registering Application services...");
            // Register services defined in the Application layer
            // Ensure E_Government.Application has a corresponding extension method (e.g., AddApplicationServices)
            // If AddApplicationService() comes from a different namespace/assembly, ensure it's referenced.
            services.AddApplicationServices(configuration); // Pass configuration
            Console.WriteLine("Mediator: Application services registered.");

            Console.WriteLine("Mediator: Registering Infrastructure services...");
            // Register services defined in the Infrastructure layer
            // This requires the Mediator project to have a project reference to the Infrastructure project.
            services.AddInfrastructureServices(configuration); // Call the extension method from E_Government.Infrastructure
            Console.WriteLine("Mediator: Infrastructure services registered.");

            // Add any Mediator-specific services here if needed

            return services;
        }

        // Note: The previous private AddInfrastructureServices method that used Assembly.Load
        // has been removed as we now directly call the method from the referenced Infrastructure project.
    }
}

