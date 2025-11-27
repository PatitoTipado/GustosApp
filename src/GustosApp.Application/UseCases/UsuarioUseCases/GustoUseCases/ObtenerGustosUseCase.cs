using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases
{
    public class ObtenerGustosUseCase
    {
        private readonly IUsuarioRepository _usuarioRepo;

        public ObtenerGustosUseCase(IUsuarioRepository usuarioRepo)
        {
            _usuarioRepo = usuarioRepo;
        }

        public virtual async Task<UsuarioPreferencias> HandleAsync(string firebaseUid, CancellationToken ct = default, List<string> gustos = null)
        {

            var usuario = await _usuarioRepo.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado o no registrado.");

            if (gustos == null || gustos != null && gustos.Count() == 0)
            {
                return new UsuarioPreferencias
                {
                    Gustos = usuario.Gustos.Select(g => g.Nombre).ToList(),
                    Restricciones = usuario.Restricciones.Select(r => r.Nombre).ToList(),
                    CondicionesMedicas = usuario.CondicionesMedicas.Select(c => c.Nombre).ToList()
                };
            }

            return new UsuarioPreferencias
            {
                Gustos = gustos,
                Restricciones = usuario.Restricciones.Select(r => r.Nombre).ToList(),
                CondicionesMedicas = usuario.CondicionesMedicas.Select(c => c.Nombre).ToList()
            };

        }

    }
}
