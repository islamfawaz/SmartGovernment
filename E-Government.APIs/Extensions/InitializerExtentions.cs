using E_Government.Domain.RepositoryContracts.Persistence;

namespace E_Government.APIs.Extensions
{
    public static class InitializerExtentions
    {
        public static async Task<WebApplication> InitializeDbAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateAsyncScope();
            var services = scope.ServiceProvider;
            var storeContextInitializer = services.GetRequiredService<IDbInitializer>();

            //Ask run time enviroment for object from store context explicitly

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
