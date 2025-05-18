// E-Government.Infrastructure/DependencyInjection.cs
using E_Government.Core.Domain.Entities; // Added for ApplicationUser
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.ServiceContracts; // Added for IBillNumberGenerator
using E_Government.Core.ServiceContracts.Common;
using E_Government.Infrastructure.EGovernment_Unified;
using E_Government.Infrastructure.Services; // Added for BillNumberGenerator implementation
using Microsoft.AspNetCore.Identity; // Added for Identity services
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
            var connectionString = configuration.GetConnectionString("EGovernment_Unified2");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'EGovernment_Unified' not found...");
            }
            else
            {
                Console.WriteLine("Registering UnifiedDbContext...");
                services.AddDbContext<UnifiedDbContext>(options =>
                    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));
                Console.WriteLine("UnifiedDbContext registered.");
            }

            // --- Register ASP.NET Core Identity ---
            Console.WriteLine("Registering ASP.NET Core Identity services...");
           

            // --- Register UnitOfWork and Generic Repository ---
            services.AddScoped<IUnitOfWork,UnitOfWork>();


            // --- Register Specific Infrastructure Services ---
            Console.WriteLine("Registering specific infrastructure services (e.g., BillNumberGenerator)...");
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBillNumberGenerator, BillNumberGenerator>(); // Added registration
   
            // TODO: Add registration for IPaymentService implementation here if not covered by marker interfaces
            Console.WriteLine("Specific infrastructure services registered.");

            // --- Register services from this assembly using Marker Interfaces ---
            // Note: If BillNumberGenerator implements IScopedService, the explicit registration above is still recommended for clarity
            // but the marker interface scan might also pick it up.
            Assembly assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine($"Scanning Infrastructure assembly ({assembly.GetName().Name}) for marker interface registrations...");
            RegisterServicesWithLifetime<ITransientService>(services, assembly, ServiceLifetime.Transient);
            RegisterServicesWithLifetime<IScopedService>(services, assembly, ServiceLifetime.Scoped);
            RegisterServicesWithLifetime<ISingletonService>(services, assembly, ServiceLifetime.Singleton);
            Console.WriteLine("Marker interface scanning complete.");

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

