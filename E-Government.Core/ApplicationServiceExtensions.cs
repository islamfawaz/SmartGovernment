// E_Government.Application/ApplicationServiceExtensions.cs
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.ServiceContracts;
using E_Government.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace E_Government.Application
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // No services to register yet, but keep the method for future use
            services.AddScoped<IBillingService, BillingService>();

            return services;
        }
    }
}