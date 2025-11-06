using GustosApp.Application.Interfaces;
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
    public class NotificacionRepositoryEF : INotificacionRepository
    {
        private readonly GustosDbContext _context;

        public NotificacionRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }

        public async Task crearAsync(Notificacion notificacion, CancellationToken cancellationToken)
        {
            await _context.Notificaciones.AddAsync(notificacion, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }



        public async Task EliminarAsync(Guid notificacionId, CancellationToken ct)
        {
            var notificacion = await _context.Notificaciones.FindAsync(notificacionId, ct);
            if (notificacion != null)
            {
                _context.Notificaciones.Remove(notificacion);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<Notificacion> GetByIdAsync(Guid notificacionId, CancellationToken ct)
        {
            var notificacion = await _context.Notificaciones
                .Include(n => n.Invitacion)
                .FirstOrDefaultAsync(n => n.Id == notificacionId, ct);

            if (notificacion == null)
                throw new KeyNotFoundException($"Notificación con ID {notificacionId} no encontrada.");

            return notificacion;
        }


        public async Task MarcarComoLeidaAsync(Guid notificacionId, CancellationToken ct)
        {
            var notificacion = await _context.Notificaciones.FindAsync(notificacionId, ct);

            if (notificacion != null && !notificacion.Leida)
            {
                notificacion.Leida = true;
                await _context.SaveChangesAsync(ct);
            }
        }


        public async Task<List<Notificacion>> ObtenerNotificacionPorUsuarioAsync(Guid usuarioId, CancellationToken ct)
        {
            return await _context.Notificaciones
                .Where(n => n.UsuarioDestinoId == usuarioId)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync(ct);
        }

        public async Task UpdateAsync(Notificacion notificacion, CancellationToken ct)
        {
            _context.Notificaciones.Update(notificacion);
            await _context.SaveChangesAsync(ct);
        }

    }
    }
