using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class RestauranteMenuRepositoryEF : IRestauranteMenuRepository
    {
        private readonly GustosDbContext _db;

        public RestauranteMenuRepositoryEF(GustosDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(RestauranteMenu menu, CancellationToken ct)
        {
            await _db.RestauranteMenus.AddAsync(menu, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(RestauranteMenu menu, CancellationToken ct)
        {
            _db.RestauranteMenus.Update(menu);
            await _db.SaveChangesAsync(ct);
        }

        public Task<RestauranteMenu?> GetByRestauranteIdAsync(Guid restauranteId, CancellationToken ct)
        {
            return _db.RestauranteMenus
                .FirstOrDefaultAsync(m => m.RestauranteId == restauranteId, ct);
        }
    }

}
