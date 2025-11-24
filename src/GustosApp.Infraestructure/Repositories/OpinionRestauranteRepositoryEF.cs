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
    public class OpinionRestauranteRepositoryEF : IOpinionRestauranteRepository
    {
        private readonly GustosDbContext _context;

        public OpinionRestauranteRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }
        public async Task CrearAsync(OpinionRestaurante opinionRestaurante, CancellationToken cancellationToken)
        {
            await _context.OpinionesRestaurantes.AddAsync(opinionRestaurante, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> ExisteValoracionAsync(Guid usuarioId, Guid restauranteId, CancellationToken cancellationToken)
        {
            return await _context.OpinionesRestaurantes
                            .AnyAsync(v => v.UsuarioId == usuarioId && v.RestauranteId == restauranteId);
        }

        public async Task<List<OpinionRestaurante>> ObtenerPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken)
         {
             return await _context.OpinionesRestaurantes
                            .Where(n => n.UsuarioId == usuarioId)
                            .Include(v => v.Restaurante)
                            .Include(v => v.Usuario) 
                            .ToListAsync(cancellationToken);
        }

        public async Task<List<OpinionRestaurante>> ObtenerPorRestauranteAsync(Guid restauranteId, CancellationToken cancellationToken)
        {
            return await _context.OpinionesRestaurantes
                .Where(v => v.RestauranteId == restauranteId)
                .ToListAsync(cancellationToken);
        }

        
    }
}
