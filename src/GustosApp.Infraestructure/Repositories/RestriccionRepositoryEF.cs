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
    public class RestriccionRepositoryEF : IRestriccionRepository
    {
        private readonly GustosDbContext _dbContext;
        public RestriccionRepositoryEF(GustosDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Restriccion>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.Restricciones.ToListAsync(ct);
        }

        public async Task<List<Restriccion>> GetRestriccionesByIdsAsync(List<Guid> ids, CancellationToken ct)
        {
            return await _dbContext.Restricciones
                .Where(r => ids.Contains(r.Id))
                .Include(r => r.TagsProhibidos)
                .ToListAsync(ct);
        }
    }
}
