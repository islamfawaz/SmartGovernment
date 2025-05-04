using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace E_Government.UI.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public CustomExceptionMiddleware(RequestDelegate next, ILogger logger)
        {

            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            // Handle exceptions here
            try
            {
               await _next(httpContext);

            }
            catch (Exception ex)
            {

                if (ex.InnerException is not null)
                {
                    _logger.LogError("{ExceptionType} {ExceptionMessage}",
                  _logger.GetType().ToString(),
                    ex.InnerException.Message);
                }
                else
                {
                    _logger.LogError("{ExceptionType} {ExceptionMessage}",
                        ex.GetType().ToString(),
                        ex.Message);
                }

                httpContext.Response.StatusCode = 550;

                await httpContext.Response.WriteAsync("An error occurred while processing your request.");


            }

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CustomExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
