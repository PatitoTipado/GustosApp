using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Infraestructure.Repositories
{
    public class ValoracionUsuarioRepositoryEF : IValoracionUsuarioRepository
    {
        private readonly GustosDbContext _context;

        public ValoracionUsuarioRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }
        public async Task CrearAsync(Valoracion valoracion, CancellationToken cancellationToken)
        {
            await _context.Valoraciones.AddAsync(valoracion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExisteValoracionAsync(Guid usuarioId, Guid restauranteId, CancellationToken cancellationToken)
        {
            return await _context.Valoraciones
                            .AnyAsync(v => v.UsuarioId == usuarioId && v.RestauranteId == restauranteId);
        }

        public async Task<List<Valoracion>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken)
         {
             return await _context.Valoraciones
                            .Where(n => n.UsuarioId == usuarioId)
                            .Include(v => v.Restaurante)
                            .Include(v => v.Usuario) 
                            .ToListAsync(cancellationToken);
        }

        public async Task<List<Valoracion>> ObtenerPorRestauranteAsync(Guid restauranteId, CancellationToken cancellationToken)
        {
            return await _context.Valoraciones
                .Where(v => v.RestauranteId == restauranteId)
                .ToListAsync(cancellationToken);
        }

    }
}
