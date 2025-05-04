using E_Government.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace E_Government.Mediator
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAllLayers(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add Core services - this depends on how you've set up your Core project
            // Make sure this method exists or is properly imported
            services.AddApplicationService();

            // Add Infrastructure services
            services.AddInfrastructureServices(configuration);

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            try
            {
                // Register CustomerWithMetersSpec with its string parameter
                var requiredParameter = configuration["RequiredStringParameter"]; // Alternative to GetValue

                // Get the type using assembly qualified name to be more specific
                var coreAssembly = Assembly.Load("E_Government.Core");
                var specType = coreAssembly.GetType("E_Government.Core.Domain.Specification.Bills.CustomerWithMetersSpec");

                if (specType != null)
                {
                    services.AddTransient(provider =>
                        Activator.CreateInstance(specType, requiredParameter)
                    );
                }

                // Load Infrastructure assembly
                var infrastructureAssembly = Assembly.Load("E_Government.Infrastructure");
                var startupType = infrastructureAssembly.GetType("E_Government.Infrastructure.StartupSetup");

                if (startupType != null)
                {
                    var setupMethod = startupType.GetMethod("AddInfrastructureToServices");
                    setupMethod?.Invoke(null, new object[] { services, configuration });
                }
                else
                {
                    // If StartupSetup doesn't exist, try the old class
                    var diType = infrastructureAssembly.GetType("E_Government.Infrastructure.DependencyInjection");
                    if (diType != null)
                    {
                        var instance = Activator.CreateInstance(diType);
                        var method = diType.GetMethod("AddInfrastructureServices");
                        method?.Invoke(instance, new object[] { services, configuration });
                    }
                }
            }
            catch (Exception ex)
            {
                // Consider logging the exception
                Console.WriteLine($"Error registering services: {ex.Message}");
            }

            return services;
        }
    }
}