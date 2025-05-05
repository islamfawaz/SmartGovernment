// E-Government.Infrastructure/DependencyInjection.cs
using E_Government.Core.Domain.RepositoryContracts.Infrastructure;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.ServiceContracts;
using E_Government.Core.ServiceContracts.Common;
using E_Government.Infrastructure.EGovernment_Unified;
using E_Government.Infrastructure.Generic_Repository; // Correct namespace for GenericRepository
using E_Government.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace E_Government.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // --- Register DbContext ---
            // Ensure you have a connection string named "DefaultConnection" in your appsettings.json
            var connectionString = configuration.GetConnectionString("EGovernment_Unified"); // Corrected name
            if (string.IsNullOrEmpty(connectionString))
            {
                // Throw an explicit exception if the connection string is missing
                throw new InvalidOperationException("Connection string 'EGovernment_Unified' not found in configuration. Please ensure it is set in appsettings.json or environment variables.");
            }
            else
            {
                Console.WriteLine("Registering UnifiedDbContext...");
                services.AddDbContext<UnifiedDbContext>(options =>
                    options.UseSqlServer(connectionString));
                Console.WriteLine("UnifiedDbContext registered.");
            }

            // --- Register UnitOfWork and Generic Repository ---
            Console.WriteLine("Registering UnitOfWork and GenericRepository...");
            // UnitOfWork is often Scoped to align with the DbContext lifetime
            services.AddScoped<IUnitOfWork, E_Government.Infrastructure.Persistence.Repository.UnitOfWork>(); // Use fully qualified name
            // GenericRepository is also typically Scoped
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Console.WriteLine("UnitOfWork and GenericRepository registered.");

            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBillNumberGenerator, BillNumberGenerator>();
            // --- Register services from this assembly using Marker Interfaces ---
            Assembly assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine($"Scanning Infrastructure assembly ({assembly.GetName().Name}) for marker interface registrations...");

            // Register Transient Services
            RegisterServicesWithLifetime<ITransientService>(services, assembly, ServiceLifetime.Transient);
            // Register Scoped Services
            RegisterServicesWithLifetime<IScopedService>(services, assembly, ServiceLifetime.Scoped);
            // Register Singleton Services
            RegisterServicesWithLifetime<ISingletonService>(services, assembly, ServiceLifetime.Singleton);

            Console.WriteLine("Marker interface scanning complete.");

            // --- Add other Infrastructure specific registrations below ---
            // Example: services.AddHttpClient(...);

            return services;
        }

        // --- Helper Methods (Unchanged) ---

        static void RegisterServicesWithLifetime<TMarkerInterface>(IServiceCollection services, Assembly assembly, ServiceLifetime lifetime)
        {
            var serviceTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(TMarkerInterface).IsAssignableFrom(t))
                .ToList();

            foreach (var implementationType in serviceTypes)
            {
                var serviceInterface = implementationType.GetInterfaces()
                    .FirstOrDefault(i => i != typeof(TMarkerInterface) && !i.IsGenericType && i.Namespace != null && i.Namespace.StartsWith("E-Government.Core"));

                if (serviceInterface == null)
                {
                    serviceInterface = implementationType.GetInterfaces()
                        .FirstOrDefault(i => i != typeof(TMarkerInterface) && !i.IsGenericType && i.Namespace != null && i.Namespace.StartsWith("E-Government.Infrastructure"));

                    if (serviceInterface == null)
                    {
                        Console.WriteLine($"Warning (Infrastructure Scan): No specific service interface found for {implementationType.Name} implementing {typeof(TMarkerInterface).Name} in expected namespaces (Core, Infrastructure). Registering concrete type.");
                        RegisterService(services, null, implementationType, lifetime);
                    }
                    else
                    {
                        RegisterService(services, serviceInterface, implementationType, lifetime);
                    }
                }
                else
                {
                    RegisterService(services, serviceInterface, implementationType, lifetime);
                }
            }
        }

        static void RegisterService(IServiceCollection services, Type? serviceType, Type implementationType, ServiceLifetime lifetime)
        {
            Type typeToRegister = serviceType ?? implementationType;
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    if (serviceType != null)
                        services.AddTransient(serviceType, implementationType);
                    else
                        services.AddTransient(implementationType);
                    break;
                case ServiceLifetime.Scoped:
                    if (serviceType != null)
                        services.AddScoped(serviceType, implementationType);
                    else
                        services.AddScoped(implementationType);
                    break;
                case ServiceLifetime.Singleton:
                    if (serviceType != null)
                        services.AddSingleton(serviceType, implementationType);
                    else
                        services.AddSingleton(implementationType);
                    break;
            }
            Console.WriteLine($"Registered from Infrastructure: {typeToRegister.FullName ?? typeToRegister.Name} -> {implementationType.FullName ?? implementationType.Name} ({lifetime})");
        }
    }
}

