using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases
{
    public class ObtenerSolicitudesRestaurantesPendientesUseCase
    {
        private readonly ISolicitudRestauranteRepository _repo;

        public ObtenerSolicitudesRestaurantesPendientesUseCase(ISolicitudRestauranteRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<SolicitudRestaurante>> HandleAsync(CancellationToken ct)
        {
            var solicitudes = await _repo.GetPendientesAsync(ct);

            if (solicitudes == null) throw new Exception("No hay lista de solicitudes pendiente ");
            return solicitudes;
        }
    }

  
}
