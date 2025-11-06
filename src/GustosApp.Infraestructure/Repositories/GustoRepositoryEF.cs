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

        public List<Gusto> ObtenerGustoPorCoincidencia(string nombreGusto)
        {
            return _dbContext.Gustos
                .Where(g => g.Nombre.ToLower().Contains(nombreGusto.ToLower()))
                .ToList();
        }

        public async Task<List<Gusto>> obtenerGustosPorNombre(List<string>gustos)
        {
            List<Gusto> resultados =await _dbContext.Gustos.ToListAsync();

            resultados = resultados.Where(g => gustos.Contains(g.Nombre)).ToList();

            return resultados;
        }

        public List<Gusto> obtenerGustosPorPaginacion(int cantidadInicio, int final)
        {
            return _dbContext.Gustos
                .OrderBy(g => g.Nombre)       // siempre ordenar para que la paginación sea consistente
                .Skip(cantidadInicio)           // offset
                .Take(final)               // limit
                .ToList();
        }
    }
}
