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

        public async Task<bool> EliminarGustosAlGrupo(Guid grupoId, List<Gusto> gustos)
        {
            int eliminados = 0;

            foreach (var gusto in gustos)
            {
                var entidad = await _context.GrupoGustos
                    .FirstOrDefaultAsync(gg => gg.GrupoId == grupoId && gg.GustoId == gusto.Id);

                if (entidad != null)
                {
                    _context.GrupoGustos.Remove(entidad);
                    eliminados++;
                }
            }

            if (eliminados == 0)
                throw new Exception("Ninguno de los gustos que intenta eliminar existe en el grupo.");

            await _context.SaveChangesAsync();
            return eliminados > 0;
        }


        public async Task<List<string>> ObtenerGustosDelGrupo(Guid grupoId)
        {
            var gustos = await _context.GrupoGustos
                .Where(gg => gg.GrupoId == grupoId)
                .Join(
                    _context.Gustos,
                    gg => gg.GustoId,
                    g => g.Id,
                    (gg, g) => g.Nombre
                )
                .ToListAsync();

            return gustos;
        }

    }
}
