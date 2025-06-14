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

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            });
            services.AddIdentityService(configuration);
            services.AddApplicationServices(configuration);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddInfrastructureServices(configuration); 
            #endregion

            var app = builder.Build();

            await app.InitializeDbAsync();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowFrontend");
            var jwtSettings = app.Services.GetRequiredService<IOptions<JwtSettings>>().Value;                 

            app.UseAuthentication(); 
            app.UseAuthorization();
            app.MapControllers();
            app.MapControllers();
            app.MapHub<DashboardHub>("/dashboardHub");
            app.UseSwagger();
            app.UseSwaggerUI();

            Console.WriteLine("API: Starting application...");
            app.Run();
        }
    }
}
