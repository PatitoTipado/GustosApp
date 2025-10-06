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
    public class GuardarCondicionesUseCase
    {
        private readonly ICondicionMedicaRepository _condiciones;
        private readonly IUsuarioRepository _user;

        public GuardarCondicionesUseCase(ICondicionMedicaRepository condiciones, IUsuarioRepository user)
        {
            _condiciones = condiciones;
            _user = user;
        }

        public async Task<GuardarCondicionesResponse> HandleAsync(string uid, List<Guid> ids, bool skip, CancellationToken ct)
        {
            if (skip) return new GuardarCondicionesResponse("Paso omitido", new List<string>());

            var usuario = await _user.GetByFirebaseUidAsync(uid, ct)
                                   ?? throw new InvalidOperationException("Usuario no encontrado.");

            usuario.CondicionesMedicas.Clear();

            var nuevas = await _condiciones.GetByIdsAsync(ids, ct);

            foreach (var condicion in nuevas)
            {
               usuario.CondicionesMedicas.Add(condicion);
            }
            var gustosRemovidos = usuario.ValidarCompatibilidad();

            usuario.AvanzarPaso(RegistroPaso.Gustos);

            await _user.SaveChangesAsync(ct);

            return new GuardarCondicionesResponse("Condiciones médicas actualizadas correctamente.", gustosRemovidos);
        }
    }
}
