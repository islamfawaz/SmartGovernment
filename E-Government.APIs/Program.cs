using E_Government.APIs.Extensions;
using E_Government.Application;
using E_Government.Domain.Helper;
using E_Government.Domain.Helper.Hub;
using E_Government.Infrastructure;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

namespace E_Government.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var configuration = builder.Configuration;

            #region Configure Service
            services.AddCorsConfiguration();   
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddIdentityService(configuration);
            services.AddApplicationServices(configuration);
            services.AddInfrastructureServices(configuration);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            #endregion

            var app = builder.Build();

            await app.InitializeDbAsync();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Use CORS before authentication/authorization
            app.UseCors("AllowFrontend");

            var jwtSettings = app.Services.GetRequiredService<IOptions<JwtSettings>>().Value;
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapHub<DashboardHub>("/dashboardHub");

            app.UseSwagger();
            app.UseSwaggerUI();

            app.Run();
        }
    }
}