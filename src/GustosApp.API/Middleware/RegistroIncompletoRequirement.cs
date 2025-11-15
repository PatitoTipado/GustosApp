using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;

namespace GustosApp.API.Middleware
{
    public class RegistroIncompletoRequirement : IAuthorizationRequirement { }

    public class RegistroIncompletoHandler : AuthorizationHandler<RegistroIncompletoRequirement>
    {
        private readonly IUsuarioRepository _usuarios;

        public RegistroIncompletoHandler(IUsuarioRepository usuarios)
        {
            _usuarios = usuarios;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RegistroIncompletoRequirement requirement)
        {
            var uid = context.User.FindFirst("user_id")?.Value;

            if (uid == null)
                return;

            var usuario = await _usuarios.GetByFirebaseUidAsync(uid);

            if (usuario != null && usuario.PasoActual != RegistroPaso.Finalizado)
            {
                context.Succeed(requirement);
            }
        }
    }

}
