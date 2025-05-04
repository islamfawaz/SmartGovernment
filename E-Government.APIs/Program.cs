using E_Government.Application;
using E_Government.Core.Domain.RepositoryContracts;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.Domain.Specification.Bills;
using E_Government.Core.ServiceContracts;
using E_Government.Core.Services;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace E_Government.APIs
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();


            // Register controllers
            builder.Services.AddControllers();

            // Register Swagger and set OpenAPI version correctly
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "E-Government API",
                    Version = "v1",
                    Description = "Documentation for the E-Government system"
                });

                // Include XML comments if available
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });




            // Register infrastructure/application dependencies
            builder.Services.AddInfrastructure(builder.Configuration); // Ensure this method is correctly implemented


            var app = builder.Build();

            // Swagger middlewares
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Government API v1");
                    options.RoutePrefix = string.Empty; // Serves Swagger UI at root (/)
                });
            }
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}
