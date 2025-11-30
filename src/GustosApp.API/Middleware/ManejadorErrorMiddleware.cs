using System.Net;
using System.Text.Json;
using GustosApp.Application.Common.Exceptions;
using GustosApp.API.DTO;

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

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
        {
            logger.LogError(ex, "❌ Error no controlado: {Message}", ex.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            // Por defecto, error interno
            var status = HttpStatusCode.InternalServerError;
            object? result = null;

            switch (ex)
            {
                case UnauthorizedAccessException:
                    status = HttpStatusCode.Unauthorized;
                    result = new { status = 401, error = "Unauthorized", message = ex.Message };
                    break;

                case ArgumentException:
                    status = HttpStatusCode.BadRequest;
                    result = new { status = 400, error = "BadRequest", message = ex.Message };
                    break;

                    
       
                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound;
                    result = new { status = 404, error = "NotFound", message = ex.Message };
                    break;

                case InvalidOperationException:
                    status = HttpStatusCode.Conflict;
                    result = new { status = 409, error = "Conflict", message = ex.Message };
                    break;

                
                case LimiteGruposAlcanzadoException limiteEx:
                    status = (HttpStatusCode)402; 
                    var beneficios = new BeneficiosPremiumDto { Precio = 9999.99m };

                    var responseObj = new LimiteGruposAlcanzadoResponse(
                        $"Has alcanzado el límite de {limiteEx.LimiteActual} grupos para usuarios del plan {limiteEx.TipoPlan}.",
                        limiteEx.TipoPlan,
                        limiteEx.LimiteActual,
                        limiteEx.GruposActuales,
                        beneficios,
                        "/api/pago/crear"
                    );

                    result = responseObj;
                    break;


                case LimiteFavoritosAlcanzadoException favEx:
                    status = (HttpStatusCode)402;

                    var beneficiosFav = new BeneficiosPremiumDto { Precio = 9999.99m };

                    var favResponse = new
                    {
                        message = $"Has alcanzado el límite de {favEx.LimiteActual} favoritos para usuarios del plan {favEx.TipoPlan}.",
                        plan = favEx.TipoPlan,
                        limite = favEx.LimiteActual,
                        actuales = favEx.FavoritosActuales,
                        beneficios = beneficiosFav,
                        linkPago = "/api/pago/crear"
                    };

                    result = favResponse;
                    break;


                default:
                    result = new
                    {
                        status = 500,
                        error = "InternalServerError",
                        message = ex.Message,
                        detail = ex.InnerException?.Message
                    };
                    break;
            }

            response.StatusCode = (int)status;

            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await response.WriteAsync(json);
        }
    }
}

