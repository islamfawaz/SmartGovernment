// E_Government.Application/ApplicationServiceExtensions.cs
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.ServiceContracts;
using E_Government.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace E_Government.Application
{
    public static class ApplicationServiceExtensions
    {
        // In the ServiceCollectionExtensions.cs file
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            // If this is defined elsewhere, make sure to add the correct using statement
            // Otherwise, you can define it here to call the original method via reflection

         
            return services;
        }
    }
}