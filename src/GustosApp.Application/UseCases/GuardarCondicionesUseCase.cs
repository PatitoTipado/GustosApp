using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task HandleAsync(string uid, List<Guid> ids, bool skip, CancellationToken ct)
        {
            if (skip)
                return;

            var existente = await _user.GetByFirebaseUidAsync(uid, ct);

            if (existente == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            var condiciones = await _condiciones.GetByIdsAsync(ids, ct);

            foreach (var condicion in condiciones)
            {
                if (!existente.CondicionesMedicas.Any(r => r.Id == condicion.Id))
                {
                    existente.CondicionesMedicas.Add(condicion);
                }
            }

            existente.AvanzarPaso(RegistroPaso.Gustos);

            await _user.SaveChangesAsync(ct);
        }
    }
}
