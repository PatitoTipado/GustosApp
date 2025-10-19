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
    public class GustoRepositoryEF : IGustoRepository
    {
        private readonly GustosDbContext _dbContext;
        public GustoRepositoryEF(GustosDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Gusto>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.Gustos
                .Include(g => g.Tags)
                .ToListAsync(ct);
        }


        public async Task<List<Gusto>> GetByIdsAsync(List<Guid> ids, CancellationToken ct)
        {
            return await _dbContext.Gustos
                .Where(r => ids.Contains(r.Id))
                .Include(r => r.Tags)
                .ToListAsync(ct);
        }
    }
}
