using GustosApp.Domain.Interfaces;
using System.Security.Claims;

namespace GustosApp.API.Middleware
{
    public class RolesMiddleware
    {
        private readonly RequestDelegate _next;

        public RolesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUsuarioRepository usuarios)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var uid = context.User.FindFirst("user_id")?.Value
                       ?? context.User.FindFirst("sub")?.Value;

                if (!string.IsNullOrEmpty(uid))
                {
                    var usuario = await usuarios.GetByFirebaseUidAsync(uid);

                    if (usuario != null)
                    {
                        var identity = (ClaimsIdentity)context.User.Identity;

                        // remover roles previos
                        var oldRoles = identity.FindAll(ClaimTypes.Role).ToList();
                        foreach (var r in oldRoles)
                            identity.RemoveClaim(r);

                        // agregar rol actual
                        identity.AddClaim(
                            new Claim(ClaimTypes.Role, usuario.Rol.ToString())
                        );
                    }
                }
            }

            await _next(context);
        }
    }

}
