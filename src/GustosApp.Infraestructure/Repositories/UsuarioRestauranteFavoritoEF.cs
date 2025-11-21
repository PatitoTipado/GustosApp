using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Infraestructure.Repositories
{
    public class UsuarioRestauranteFavoritoEF : IUsuarioRestauranteFavoritoRepository
    {
        private readonly GustosDbContext _dbContext;

        public UsuarioRestauranteFavoritoEF(GustosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CountByUsuarioAsync(Guid usuarioId, CancellationToken ct)
        {
            return await _dbContext.UsuarioRestauranteFavoritos
                .CountAsync(x => x.UsuarioId == usuarioId, ct);
        }

        public async Task CrearAsync(UsuarioRestauranteFavorito usuarioRestauranteFavorito, CancellationToken ct)
        {
            await _dbContext.UsuarioRestauranteFavoritos.AddAsync(usuarioRestauranteFavorito, ct);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(Guid usuarioId, Guid restauranteId, CancellationToken ct)
        {
           return await _dbContext.UsuarioRestauranteFavoritos
                .AnyAsync(x => x.UsuarioId == usuarioId && x.RestauranteId == restauranteId, ct);
        }
    }
   
}
