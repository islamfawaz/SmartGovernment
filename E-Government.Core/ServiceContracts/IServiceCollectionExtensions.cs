using Microsoft.Extensions.DependencyInjection;

namespace E_Government.Core.ServiceContracts
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