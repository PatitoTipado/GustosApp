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
                           .Include(r => r.Especialidad)
                           .ToListAsync(ct);
        }


        public async Task AddAsync(Restaurante restaurante, CancellationToken ct)
            => await _context.Restaurantes.AddAsync(restaurante, ct);

        public async Task SaveChangesAsync(CancellationToken ct)
            => await _context.SaveChangesAsync(ct);
    }
}
