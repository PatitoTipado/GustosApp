using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases
{
    public class ObtenerCondicionesMedicasUseCase
    {
        public readonly ICondicionMedicaRepository _condicion;
        public readonly IUsuarioRepository _usuarios;

        public ObtenerCondicionesMedicasUseCase(ICondicionMedicaRepository condicion, IUsuarioRepository usuarios)
        {
            _condicion = condicion;
            _usuarios = usuarios;
        }

        public async Task<(IEnumerable<CondicionMedica> Todas, IEnumerable<Guid> Seleccionadas)>
            HandleAsync(string firebaseUid, CancellationToken ct = default)
        {
            var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new InvalidOperationException("Usuario no encontrado.");

            var todas = await _condicion.GetAllAsync(ct);
            var seleccionadas = usuario.CondicionesMedicas?.Select(c => c.Id).ToHashSet()
                                ?? new HashSet<Guid>();

            return (todas, seleccionadas);
        }
    }
}
