using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class RestauranteEstadisticasRepositoryEF : IRestauranteEstadisticasRepository
    {
        private readonly GustosDbContext _dbContext;

        public RestauranteEstadisticasRepositoryEF(GustosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RestauranteEstadisticas?> ObtenerPorRestauranteAsync(Guid restauranteId, CancellationToken ct = default)
        {
            return await _dbContext.RestauranteEstadisticas
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.RestauranteId == restauranteId, ct);
        }

        public async Task IncrementarTop3IndividualAsync(IEnumerable<Guid> restauranteIds, CancellationToken ct = default)
        {
            await IncrementarContadorAsync(restauranteIds, e => e.IncrementarTop3Individual(), ct);
        }

        public async Task IncrementarTop3GrupoAsync(IEnumerable<Guid> restauranteIds, CancellationToken ct = default)
        {
            await IncrementarContadorAsync(restauranteIds, e => e.IncrementarTop3Grupo(), ct);
        }

        public async Task IncrementarVisitaPerfilAsync(Guid restauranteId, CancellationToken ct = default)
        {
            await IncrementarContadorAsync(new[] { restauranteId }, e => e.IncrementarVisitaPerfil(), ct);
        }

        private async Task IncrementarContadorAsync(
            IEnumerable<Guid> restauranteIds,
            Action<RestauranteEstadisticas> accion,
            CancellationToken ct)
        {
            var ids = restauranteIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (!ids.Any())
                return;

            var existentes = await _dbContext.RestauranteEstadisticas
                .Where(e => ids.Contains(e.RestauranteId))
                .ToListAsync(ct);

            var existentesPorId = existentes.ToDictionary(e => e.RestauranteId, e => e);

            foreach (var id in ids)
            {
                if (!existentesPorId.TryGetValue(id, out var estadisticas))
                {
                    estadisticas = new RestauranteEstadisticas
                    {
                        RestauranteId = id,
                        FechaCreacionUtc = DateTime.UtcNow,
                        UltimaActualizacionUtc = DateTime.UtcNow
                    };

                    accion(estadisticas);
                    _dbContext.RestauranteEstadisticas.Add(estadisticas);
                    existentesPorId[id] = estadisticas;
                }
                else
                {
                    accion(estadisticas);
                    estadisticas.UltimaActualizacionUtc = DateTime.UtcNow;
                }
            }

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
