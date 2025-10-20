using System.Net;
using System.Text.Json;

namespace GustosApp.API.Middleware
{
    public class ManejadorErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejadorErrorMiddleware> _logger;

        public ManejadorErrorMiddleware(RequestDelegate next, ILogger<ManejadorErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
        {
            logger.LogError(ex, "❌ Error no controlado: {Message}", ex.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            var status = ex switch
            {
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                ArgumentException => HttpStatusCode.BadRequest,
                InvalidOperationException => HttpStatusCode.Conflict,
                KeyNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            };

            response.StatusCode = (int)status;

            var result = JsonSerializer.Serialize(new
            {
                status = (int)status,
                error = status.ToString(),
                message = ex.Message,
                detail = ex.InnerException?.Message
            });

            return response.WriteAsync(result);
        }
    }
}

