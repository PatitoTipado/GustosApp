using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Infraestructure.Repositories
{
    public class RestauranteRepositoryEF : IRestaurantRepository
    {
        private readonly GustosDbContext _dbContext;

        public RestauranteRepositoryEF(GustosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Restaurante>> GetAllAsync(CancellationToken ct= default)
        {
            return await _dbContext.Restaurante 
                           .Include(r => r.Especialidad)
                           .ToListAsync(ct);
        }
    }
}
