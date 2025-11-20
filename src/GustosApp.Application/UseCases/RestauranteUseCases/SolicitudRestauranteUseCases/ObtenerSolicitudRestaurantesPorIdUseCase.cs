using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases
{
    public class ObtenerSolicitudRestaurantesPorIdUseCase
    {
        private readonly ISolicitudRestauranteRepository _repo;

        public ObtenerSolicitudRestaurantesPorIdUseCase(ISolicitudRestauranteRepository repo)
        {
            _repo = repo;
        }

        public async Task<SolicitudRestaurante?> HandleAsync(Guid id, CancellationToken ct)
        {
            var solicitud = await _repo.GetByIdAsync(id, ct);

            if (solicitud == null) throw new KeyNotFoundException("La solicitud de restaurante no existe.");
            return solicitud;

                }
    }
}
