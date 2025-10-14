using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class RestauranteRepositoryEF : IRestauranteRepository
    {
        private readonly GustosDbContext _context;

        public RestauranteRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }

        public async Task<Restaurante?> GetByPlaceIdAsync(string placeId, CancellationToken ct)
        {
            return await _context.Restaurantes
                .Include(r => r.Reviews) 
                .FirstOrDefaultAsync(r => r.PlaceId == placeId, ct);
        }
        public async Task<List<Restaurante>> GetAllAsync(CancellationToken ct= default)
        {
            return await _context.Restaurantes 
                           .Include(r => r.GustosQueSirve)
                           .ToListAsync(ct);
        }


        public async Task AddAsync(Restaurante restaurante, CancellationToken ct)
            => await _context.Restaurantes.AddAsync(restaurante, ct);

        public async Task SaveChangesAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct);

        public Task<List<Restaurante>> buscarRestauranteParaUsuariosConGustosYRestricciones(
            List<string> gustos,
            List<string> restricciones,
            CancellationToken ct = default)
        {
            var gustosNormalizados = gustos.Select(g => g.ToLower()).ToList();
            var restriccionesNormalizadas = restricciones.Select(r => r.ToLower()).ToList();

            var query = _context.Restaurantes.AsQueryable();

            if (gustosNormalizados.Any())
            {
                query = query.Where(r => r.GustosQueSirve
                    .Any(g => gustosNormalizados.Contains(g.Nombre.ToLower())));
            }

            if (restriccionesNormalizadas.Any())
            {
                query = query.Where(r => r.RestriccionesQueRespeta
                    .Any(res => restriccionesNormalizadas.Contains(res.Nombre.ToLower())));
            }
            query = query
                .Include(r => r.Reviews)
                .Include(r => r.RestriccionesQueRespeta)
                .Include(r => r.Platos)
                .Include(r => r.GustosQueSirve);

            return query.ToListAsync(ct);
        }

    }
}
