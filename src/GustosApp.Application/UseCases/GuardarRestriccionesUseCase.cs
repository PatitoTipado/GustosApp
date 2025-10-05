using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class GuardarRestriccionesUseCase
    {
        private readonly IUsuarioRepository _user;
        private readonly IRestriccionRepository _restricciones;

        public GuardarRestriccionesUseCase(IUsuarioRepository user, IRestriccionRepository restricciones)
        {
            _user = user;
            _restricciones = restricciones;
        }

        public async Task HandleAsync(string uid, List<Guid> ids, bool skip, CancellationToken ct)
        {
            if (skip)
           
                return;

            var existente = await _user.GetByFirebaseUidAsync(uid, ct);

            if (existente == null)
            {
                throw new Exception("Usuario no encontrado");
            }
            // Cargar restricciones existentes
            var restricciones = await _restricciones.GetRestriccionesByIdsAsync(ids, ct);

            foreach (var restriccion in restricciones)
            {
                if (!existente.Restricciones.Any(r => r.Id == restriccion.Id))
                {
                    existente.Restricciones.Add(restriccion);
                }
            }

            existente.AvanzarPaso(RegistroPaso.Condiciones);

            await _user.SaveChangesAsync(ct);
        }
    }
}
