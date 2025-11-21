using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IRestauranteEstadisticasRepository
    {
        Task<RestauranteEstadisticas?> ObtenerPorRestauranteAsync(Guid restauranteId, CancellationToken ct = default);

        Task IncrementarTop3IndividualAsync(IEnumerable<Guid> restauranteIds, CancellationToken ct = default);

        Task IncrementarTop3GrupoAsync(IEnumerable<Guid> restauranteIds, CancellationToken ct = default);

        Task IncrementarVisitaPerfilAsync(Guid restauranteId, CancellationToken ct = default);
    }
}
