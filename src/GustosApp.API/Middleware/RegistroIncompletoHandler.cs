using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace GustosApp.API.Middleware
{
    using Microsoft.AspNetCore.Authorization;

    public class RegistroIncompletoHandler
        : AuthorizationHandler<RegistroIncompletoRequirement>
    {
        private readonly ICacheService _cache;
        private readonly IUsuarioRepository _usuarios;

        public RegistroIncompletoHandler(ICacheService cache, IUsuarioRepository usuarios)
        {
            _cache = cache;
            _usuarios = usuarios;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RegistroIncompletoRequirement requirement)
        {
            var uid = context.User.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(uid))
                return;

            string key = $"registro:{uid}:inicialCompleto";

            // 1) Busco en cache
            var cached = await _cache.GetAsync<bool?>(key);

            bool registroInicialCompleto;

            if (cached.HasValue)
            {
                registroInicialCompleto = cached.Value;
            }
            else
            {
                //  Fallback a DB (redis vacío o reiniciado)
                var usuario = await _usuarios.GetByFirebaseUidAsync(uid);
                registroInicialCompleto = usuario?.RegistroInicialCompleto ?? false;

                // Repoblar cache para futuro
                await _cache.SetAsync(key, registroInicialCompleto, TimeSpan.FromHours(12));
            }

            
            //   Si NO completó el registro inicial → permitir
            if (!registroInicialCompleto)
            {
                context.Succeed(requirement);
            }
        }
    }


}
