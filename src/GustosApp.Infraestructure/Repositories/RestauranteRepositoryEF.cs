using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class RestauranteRepositoryEF : IRestauranteRepository
    {
        private readonly GustosDbContext _db;
        public RestauranteRepositoryEF(GustosDbContext db) => _db = db;

        public async Task<Restaurante?> GetByPlaceIdAsync(string placeId, CancellationToken ct = default)
            => await _db.Restaurantes.AsNoTracking().FirstOrDefaultAsync(r => r.PlaceId == placeId, ct);

        public async Task AddAsync(Restaurante r, CancellationToken ct = default)
            => await _db.Restaurantes.AddAsync(r, ct);

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public async Task<List<Restaurante>> GetNearbyAsync(double lat, double lng, int radiusMeters, TimeSpan? maxAge = null, CancellationToken ct = default)
        {
            // Aproximación por bounding box
            double degLat = radiusMeters / 111_000.0;
            double degLng = radiusMeters / (111_000.0 * Math.Cos(lat * Math.PI / 180.0));

            var minLat = lat - degLat;
            var maxLat = lat + degLat;
            var minLng = lng - degLng;
            var maxLng = lng + degLng;

            var q = _db.Restaurantes.AsNoTracking()
                .Where(r => r.Latitud >= minLat && r.Latitud <= maxLat
                         && r.Longitud >= minLng && r.Longitud <= maxLng);

            if (maxAge.HasValue)
            {
                var threshold = DateTime.UtcNow - maxAge.Value;
                q = q.Where(r => r.UltimaActualizacion >= threshold);
            }

            // Orden simple por distancia Manhattan aproximada
            q = q.OrderBy(r => Math.Abs(r.Latitud - lat) + Math.Abs(r.Longitud - lng))
                 .Take(200);

            return await q.ToListAsync(ct);
        }

        
                public async Task<List<Restaurante>> GetAllAsync(CancellationToken ct= default)
        {
            return await _db.Restaurantes 
                           .Include(r => r.Especialidad)
                           .ToListAsync(ct);
        }

    }
}
