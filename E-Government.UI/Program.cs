using E_Government.Application;
using E_Government.Core.Domain.RepositoryContracts.Infrastructure;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using E_Government.Core.Services;
using E_Government.Infrastructure;
using E_Government.Infrastructure.EGovernment_Unified;
using E_Government.Infrastructure.Services;
using E_Government.Infrastructure.UnitOfWork;
using E_Government.UI.Extension;
using System.Text.Json.Serialization;

namespace E_Government.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddEndpointsApiExplorer();
            builder.services.addswaggergen(c =>
            {
                c.swaggerdoc("v1", new openapiinfo
                {
                    title = "e-government api",
                    version = "v1",
                    description = "api documentation",
                    contact = new openapicontact { name = "support", email = "support@egovernment.com" }
                });
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    builder => builder.WithOrigins("http://localhost:5175")
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            builder.Services.AddScoped<ILogger,Logger<Program>>();
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));


            builder.Services.AddScoped<IDbInitializer, ApplicationDbInitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IBillingService, BillingService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            // This single call registers everything
            builder.Services
                .AddApplication()
                .AddInfrastructure(builder.Configuration);

            var app = builder.Build();
            // Configure pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI();
                app.UseCors("AllowReactApp");

            }


            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            await app.InitializDbAsync();
            app.Run();
        }
    }
}