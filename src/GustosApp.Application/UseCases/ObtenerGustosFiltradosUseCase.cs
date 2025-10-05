using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Services;

namespace GustosApp.Application.UseCases
{
    public class ObtenerGustosFiltradosUseCase
    {

        private readonly IUsuarioRepository _usuarios;
        private readonly IGustoRepository _gustos;
        private readonly CompatibilidadAlimentariaService _compat;

        public ObtenerGustosFiltradosUseCase(IUsuarioRepository usuarios,
                                             IGustoRepository gustos,
                                             CompatibilidadAlimentariaService compat)
        {
            _usuarios = usuarios;
            _gustos = gustos;
            _compat = compat;
        }

    

        /* public async Task<GustosFiltradosResponse> HandleAsync(string firebaseUid, CancellationToken ct)
         {
             var usuario = await _usuarios.GetByFirebaseUidAsync(firebaseUid, ct);
             if (usuario is null) throw new AppException("Usuario no encontrado");

             var todos = await _gustos.GetAllAsync(ct);  
             var (validos, conflictos) = _compat.FiltrarGustos(todos, usuario.Restricciones, usuario.CondicionesMedicas);

             return new GustosFiltradosResponse(
                 validos.Select(g => new GustoDto(g.Id, g.Nombre, g.ImagenUrl)).ToList(),
                 conflictos.Select(c => new GustoConflictoDto(c.gusto.Id, c.gusto.Nombre, c.motivo)).ToList()
             );
         }
        */
    }
}
