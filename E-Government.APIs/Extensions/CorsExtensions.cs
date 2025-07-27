namespace E_Government.APIs.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5173",
                            "http://127.0.0.1:5500",
                            "http://localhost:5500"
                          )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return services;
        }
    }
}
