using Microsoft.Extensions.DependencyInjection;

namespace E_Government.Application.ServiceContracts
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            // Register any core services here
            return services;
        }
    }
}