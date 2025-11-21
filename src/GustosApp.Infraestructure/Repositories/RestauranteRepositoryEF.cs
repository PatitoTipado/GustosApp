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

        public async Task<Restaurante?> GetRestauranteByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.Restaurantes.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task AddAsync(Restaurante r, CancellationToken ct = default)
            => await _db.Restaurantes.AddAsync(r, ct);

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public async Task<List<Restaurante>> GetNearbyAsync(
    double lat,
    double lng,
    int radiusMeters,
    TimeSpan? maxAge = null,
    CancellationToken ct = default)
        {
            // Traer todos los restaurantes con gustos y restricciones
            var restaurantes = await _db.Restaurantes
                .AsNoTracking()
                .Include(r => r.GustosQueSirve)
                .Include(r => r.RestriccionesQueRespeta)
                .ToListAsync(ct);

            // Aproximación por bounding box
            double degLat = radiusMeters / 111_000.0;
            double degLng = radiusMeters / (111_000.0 * Math.Cos(lat * Math.PI / 180.0));

            var minLat = lat - degLat;
            var maxLat = lat + degLat;
            var minLng = lng - degLng;
            var maxLng = lng + degLng;

            // Filtrar en memoria
            var filtered = restaurantes
                .Where(r => r.Latitud >= minLat && r.Latitud <= maxLat
                         && r.Longitud >= minLng && r.Longitud <= maxLng);

            if (maxAge.HasValue)
            {
                var threshold = DateTime.UtcNow - maxAge.Value;
                filtered = filtered.Where(r => r.UltimaActualizacion >= threshold);
            }

            // Ordenar por distancia Manhattan aproximada y tomar 200
            filtered = filtered
                .OrderBy(r => Math.Abs(r.Latitud - lat) + Math.Abs(r.Longitud - lng))
                .Take(200);

            return filtered.ToList();
        }

        public async Task<List<Restaurante>> GetAllAsync(CancellationToken ct= default)
        {
            return await _db.Restaurantes 
                           .Include(r => r.GustosQueSirve)
                           .ToListAsync(ct);
        }

        public async Task<List<Restaurante>> buscarRestauranteParaUsuariosConGustosYRestricciones(
            List<string> gustos,
            List<string> restricciones,
            CancellationToken ct = default)
        {
            var gustosNormalizados = gustos.Select(g => g.ToLower()).ToList();
            var restriccionesNormalizadas = restricciones.Select(r => r.ToLower()).ToList();

            // 1. CARGA COMPLETA Y MATERIALIZACIÓN TEMPRANA (¡Aquí forzamos el ToList!)
            // Esto trae TODOS los restaurantes y sus colecciones a la memoria del servidor.
            var todosLosRestaurantes = await _db.Restaurantes
                .Include(r => r.Reviews)
                .Include(r => r.RestriccionesQueRespeta)
                .Include(r => r.Platos)
                .Include(r => r.GustosQueSirve)
                .ToListAsync(ct); // <-- El ToList() se ejecuta aquí.

            // A partir de aquí, el filtrado se realiza en la memoria de .NET (LINQ to Objects).

            var query = todosLosRestaurantes.AsEnumerable(); // Usa AsEnumerable para claridad en el filtro de memoria

            // 2. FILTRADO POR GUSTOS (INCLUSIÓN)
            if (gustosNormalizados.Any())
            {
                // El restaurante debe servir AL MENOS UN gusto que el usuario quiere.
                query = query.Where(r => r.GustosQueSirve
                    .Any(g => g.Nombre != null && gustosNormalizados.Contains(g.Nombre.ToLower())));
            }

            // 3. FILTRADO POR RESTRICCIONES (EXCLUSIÓN - Lógica Corregida en Memoria)
            if (restriccionesNormalizadas.Any())
            {
                // El restaurante NO debe respetar NINGUNA de las restricciones que tiene el usuario.
                query = query.Where(r => r.RestriccionesQueRespeta
                    .Any(res => res.Nombre != null && restriccionesNormalizadas.Contains(res.Nombre.ToLower())));
            }

            // 4. Devolver la lista filtrada en memoria.
            return query.ToList();
        }

        public async Task ActualizarValoracionAsync(Guid restauranteId, double promedio, CancellationToken cancellationToken)
        {
            var restaurante = await _db.Restaurantes.FindAsync(new object[] { restauranteId }, cancellationToken);
            if (restaurante != null)
            {
                restaurante.Valoracion = (decimal)promedio;
                await _db.SaveChangesAsync(cancellationToken);
            }

        }
        public async Task<List<Restaurante>> ObtenerRestaurantesPorGustosGrupo(
            List<Guid> gustosIds,
            CancellationToken ct = default)
        {
            if (!gustosIds.Any())
            {
                return new List<Restaurante>();
            }

            // Obtener restaurantes que sirvan al menos uno de los gustos del grupo
            var restaurantes = await _db.Restaurantes
                .Include(r => r.GustosQueSirve)
                .Include(r => r.RestriccionesQueRespeta)
                .Include(r => r.Reviews)
                .Include(r => r.Platos)
                .Where(r => r.GustosQueSirve.Any(g => gustosIds.Contains(g.Id)))
                .ToListAsync(ct);

            return restaurantes;
        }

        public async Task<List<Restaurante>> BuscarPorTextoAsync(string texto, CancellationToken ct = default)
        {
            texto = texto.ToLower();

            var restaurantes = await _db.Restaurantes
                .AsNoTracking()
                .Where(r =>
                    r.Nombre.ToLower().Contains(texto) ||
                    r.NombreNormalizado.ToLower().Contains(texto) ||
                    r.Categoria.ToLower().Contains(texto)
                )
                .ToListAsync(ct);

            // Ordenamiento por rating y coincidencia de texto
            var restaurantesOrdenados = restaurantes
                .Select(r => new
                {
                    Restaurante = r,
                    Prioridad = r.Nombre.ToLower().Contains(texto) ? 3 :
                                r.NombreNormalizado.ToLower().Contains(texto) ? 2 :
                                r.Categoria.ToLower().Contains(texto) ? 1 : 0
                })
                .OrderByDescending(x => x.Restaurante.Rating) // primero rating
                .ThenByDescending(x => x.Prioridad)          // luego coincidencia de texto
                .Select(x => x.Restaurante)
                .ToList();

            return restaurantesOrdenados;
        }


        public async Task<Restaurante?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Restaurantes.FirstOrDefaultAsync(r => r.Id == id);

        }

        public Task UpdateAsync(Restaurante restaurante, CancellationToken ct)
        {
          _db.Restaurantes.Update(restaurante);
            return _db.SaveChangesAsync(ct);
        }
    }
}

