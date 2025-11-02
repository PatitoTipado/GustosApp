using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Infraestructure.Repositories
{
    public class ReseñaRepositoryEF : IReseñaRepository
    {
        private readonly GustosDbContext _context;
        public ReseñaRepositoryEF(GustosDbContext context) => _context = context;

        public async Task RemoveByRestauranteIdAsync(Guid restauranteId, CancellationToken ct)
        {
            var reseñas = _context.ReviewsRestaurantes.Where(r => r.RestauranteId == restauranteId);
            _context.ReviewsRestaurantes.RemoveRange(reseñas);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AddAsync(ReseñaRestaurante reseña, CancellationToken ct)
        {
            await _context.ReviewsRestaurantes.AddAsync(reseña, ct);
        }
    }

}
