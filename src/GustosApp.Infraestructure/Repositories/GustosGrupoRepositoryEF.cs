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
    public class GustosGrupoRepositoryEF : IGustosGrupoRepository
    {

        private GustosDbContext _context;

        public GustosGrupoRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AgregarGustosAlGrupo(Guid grupoId, List<Gusto> gustos)
        {
            var gustosExistentes = await _context.GrupoGustos
                .Where(gg => gg.GrupoId == grupoId)
                .Select(gg => gg.GustoId)
                .ToListAsync();

            var nuevosGustos = gustos
                .Where(g => !gustosExistentes.Contains(g.Id))
                .Select(g => new GrupoGusto
                {
                    Id = Guid.NewGuid(),
                    GrupoId = grupoId,
                    GustoId = g.Id
                })
                .ToList();

            if (!nuevosGustos.Any())
                return true;

            await _context.GrupoGustos.AddRangeAsync(nuevosGustos);

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
