using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
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

        public async Task<GuardarRestriccionesResponse> HandleAsync(string uid, List<Guid> ids, bool skip, CancellationToken ct)
        {
           
            if (skip)
                return new GuardarRestriccionesResponse("Paso omitido", new List<string>());


            var usuario = await _user.GetByFirebaseUidAsync(uid, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");

         
            usuario.Restricciones.Clear();

            var nuevas = await _restricciones.GetRestriccionesByIdsAsync(ids, ct);

            foreach (var restriccion in nuevas)
                usuario.Restricciones.Add(restriccion);


            var gustosRemovidos = usuario.ValidarCompatibilidad();

            usuario.AvanzarPaso(RegistroPaso.Condiciones);

            await _user.SaveChangesAsync(ct);

            return new GuardarRestriccionesResponse(
                "Restricciones actualizadas correctamente.",
                gustosRemovidos
            );
        }
    }
}
