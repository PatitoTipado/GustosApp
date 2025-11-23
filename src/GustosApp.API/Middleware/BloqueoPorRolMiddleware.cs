//*using GustosApp.Domain.Model.@enum;
/*
namespace GustosApp.API.Middleware
{
    public class BloqueoPorRolMiddleware
    {
        private readonly RequestDelegate _next;

        public BloqueoPorRolMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.User.Identity?.IsAuthenticated ?? false)
            {
                await _next(context);
                return;
            }

            var rol = context.User.Claims.FirstOrDefault(c => c.Type == "rol")?.Value;

         
            if (rol == RolUsuario.PendienteRestaurante.ToString())
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Tu solicitud de restaurante está en revisión.",
                    code = "PENDIENTE"
                });
                return;
            }

            //BLOQUEAR DUEÑOS (excepto rutas especiales)
            if (rol == RolUsuario.DuenoRestaurante.ToString())
            {
                var path = context.Request.Path.Value?.ToLower() ?? "";

                //ejmplo de rutas permitidas
                var allowed = new[] {
                "/api/restaurantes/mio",
                "/api/restaurantes/menu",
                "/api/restaurantes/imagenes",
                "/api/restaurantes/dashboard"
            };

                bool esPermitida = allowed.Any(p => path.StartsWith(p));

                if (!esPermitida)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Tu cuenta pertenece a un restaurante. Acceso restringido.",
                        code = "DUENO"
                    });
                    return;
                }
            }

            await _next(context);
        }
    }
}
*/