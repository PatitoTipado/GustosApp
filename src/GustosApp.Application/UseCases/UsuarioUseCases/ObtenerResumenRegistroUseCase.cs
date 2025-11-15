using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases
{
    public class ObtenerResumenRegistroUseCase
    {
        public readonly IUsuarioRepository _usuarios;
        private readonly ICacheService _cache;
        public ObtenerResumenRegistroUseCase(IUsuarioRepository usuarios, ICacheService cache)
        {
            _usuarios = usuarios;
            _cache = cache;
        }

        public async Task<Usuario> HandleAsync(string uid, CancellationToken ct)
        {
            // 1. Obtener usuario desde BD
            var usuario = await _usuarios.GetByFirebaseUidAsync(uid, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");

            // 2. Intentar obtener progresos desde Redis
            var restricciones = await _cache.GetAsync<List<Guid>>($"registro:{uid}:restricciones");
            var condiciones = await _cache.GetAsync<List<Guid>>($"registro:{uid}:condiciones");
            var gustos = await _cache.GetAsync<List<Guid>>($"registro:{uid}:gustos");

            // 3. Sobrescribir el estado del usuario con cache si existe
            if (restricciones != null)
                usuario.Restricciones = usuario.Restricciones.Where(r => restricciones.Contains(r.Id)).ToList();

            if (condiciones != null)
                usuario.CondicionesMedicas = usuario.CondicionesMedicas.Where(c => condiciones.Contains(c.Id)).ToList();

            if (gustos != null)
                usuario.Gustos = usuario.Gustos.Where(g => gustos.Contains(g.Id)).ToList();

            return usuario;
        }
    }

}

