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
     public class ObtenerCondicionesMedicasUseCase
    {
        public readonly ICondicionMedicaRepository _condicion;
        public readonly IUsuarioRepository _usuarios;

        public ObtenerCondicionesMedicasUseCase(ICondicionMedicaRepository condicion, IUsuarioRepository usuarios)
        {
            _condicion = condicion;
            _usuarios = usuarios;
        }

        public async Task<List<CondicionMedicaResponse>> HandleAsync(string uidFirebase,CancellationToken ct = default)
        {
            var usuario = await _usuarios.GetByFirebaseUidAsync(uidFirebase, ct)
                      ?? throw new InvalidOperationException("Usuario no encontrado.");

            var todas= await _condicion.GetAllAsync(ct);

            var seleccionadas = usuario.CondicionesMedicas?
                .Select(c => c.Id)
                .ToHashSet() ?? new HashSet<Guid>();


            var resultado = todas.Select(r => new CondicionMedicaResponse
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Seleccionado = seleccionadas.Contains(r.Id)
            }).ToList();



            return resultado;
        }
    }
}
