public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register other services...

        // Register IBillingService
        services.AddScoped<IBillingService, BillingService>();

        return services;
    }
}
