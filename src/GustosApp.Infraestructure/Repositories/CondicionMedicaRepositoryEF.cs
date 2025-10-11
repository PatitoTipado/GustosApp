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
    public class CondicionMedicaRepositoryEF : ICondicionMedicaRepository
    {
        private readonly GustosDbContext _dbContext;
        public CondicionMedicaRepositoryEF(GustosDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<CondicionMedica>> GetAllAsync(CancellationToken ct)
        {
            return await _dbContext.CondicionesMedicas.ToListAsync(ct);
        }

        public async Task<List<CondicionMedica>> GetByIdsAsync(List<Guid> ids, CancellationToken ct)
        {
            return await _dbContext.CondicionesMedicas
                .Where(r => ids.Contains(r.Id))
                .Include(r => r.TagsCriticos)
                .ToListAsync(ct);
        }
    }
}
