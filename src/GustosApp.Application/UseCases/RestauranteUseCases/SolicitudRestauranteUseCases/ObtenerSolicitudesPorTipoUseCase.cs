using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases
{
    public class ObtenerSolicitudesPorTipoUseCase
    {
        private readonly ISolicitudRestauranteRepository _repo;

        public ObtenerSolicitudesPorTipoUseCase(ISolicitudRestauranteRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<SolicitudRestaurante>> HandleAsync(
            EstadoSolicitudRestaurante filtro,
            CancellationToken ct)
        {
            return filtro switch
            {
                EstadoSolicitudRestaurante.Pendiente =>
                    await _repo.GetByEstadoAsync(EstadoSolicitudRestaurante.Pendiente, ct),

                EstadoSolicitudRestaurante.Aprobada =>
                    await _repo.GetByEstadoAsync(EstadoSolicitudRestaurante.Aprobada, ct),

                EstadoSolicitudRestaurante.Rechazada =>
                    await _repo.GetByEstadoAsync(EstadoSolicitudRestaurante.Rechazada, ct),

                _ => await _repo.GetAllAsync(ct)
            };
        }
    }

}
