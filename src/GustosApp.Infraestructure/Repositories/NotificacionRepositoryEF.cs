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
    public class NotificacionRepositoryEF : INotificacionRepository
    {
        private readonly GustosDbContext _context;

        public NotificacionRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }

        public async Task crearAsync(Notificacion notificacion, CancellationToken cancellationToken)
        {
           await _context.Notificaciones.AddAsync(notificacion,cancellationToken);
           await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task MarcarComoLeidaAsync(Guid usuarioId, CancellationToken ct)
        {
            var notificaciones = await _context.Notificaciones
                .Where(n => n.UsuarioDestinoId == usuarioId && !n.Leida)
                .ToListAsync(ct);

            foreach (var n in notificaciones)
            {
                n.Leida = true;
            }

            await _context.SaveChangesAsync(ct);
        }

        public async Task <List<Notificacion>> ObtenerNotificacionPorUsuarioAsync(Guid usuarioId, CancellationToken ct)
        {
            return await _context.Notificaciones
                .Where(n => n.UsuarioDestinoId == usuarioId)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync(ct);
        }
    }
}
