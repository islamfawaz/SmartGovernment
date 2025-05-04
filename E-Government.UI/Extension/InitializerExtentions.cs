using E_Government.Core.Domain.RepositoryContracts.Persistence;
using Microsoft.EntityFrameworkCore.Internal;

namespace E_Government.UI.Extension
{
    public static class InitializerExtentions
    {
        public static async Task<WebApplication> InitializDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var services = scope.ServiceProvider;
            var storeContextInitializer = services.GetRequiredService<IDbInitializer>();

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await storeContextInitializer.InitializerAsync();
                await storeContextInitializer.SeedAsync();

                
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "error has been occur during apply migration");

                throw;
            }

            return app;

        }



    }
}
