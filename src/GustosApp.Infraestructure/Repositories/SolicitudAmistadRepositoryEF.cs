using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using GustosApp.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class SolicitudAmistadRepositoryEF : ISolicitudAmistadRepository
    {
        private readonly GustosDbContext _context;

        public SolicitudAmistadRepositoryEF(GustosDbContext context) => _context = context;

        public async Task<SolicitudAmistad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _context.SolicitudesAmistad
                .Include(s => s.Remitente)
                .Include(s => s.Destinatario)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task<IEnumerable<SolicitudAmistad>> GetSolicitudesPendientesByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
             => await _context.SolicitudesAmistad
                 .Include(s => s.Remitente)
                 .Include(s => s.Destinatario)
                 .Where(s => s.DestinatarioId == usuarioId && s.Estado == EstadoSolicitud.Pendiente)
                 .ToListAsync(cancellationToken);

        public async Task<IEnumerable<SolicitudAmistad>> GetSolicitudesEnviadasByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
            => await _context.SolicitudesAmistad
                .Include(s => s.Remitente)
                .Include(s => s.Destinatario)
                .Where(s => s.RemitenteId == usuarioId)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Usuario>> GetAmigosByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            var aceptadas = await _context.SolicitudesAmistad
                .Include(s => s.Remitente)
                .Include(s => s.Destinatario)
                .Where(s => (s.RemitenteId == usuarioId || s.DestinatarioId == usuarioId) && s.Estado == EstadoSolicitud.Aceptada)
                .ToListAsync(cancellationToken);

            var amigos = aceptadas.Select(s => s.RemitenteId == usuarioId ? s.Destinatario : s.Remitente).ToList();
            return amigos;
        }

        public async Task<bool> ExisteSolicitudPendienteAsync(Guid remitenteId, Guid destinatarioId, CancellationToken cancellationToken = default)
            => await _context.SolicitudesAmistad.AnyAsync(s => s.RemitenteId == remitenteId && s.DestinatarioId == destinatarioId && s.Estado == EstadoSolicitud.Pendiente, cancellationToken);

        public async Task<SolicitudAmistad> CreateAsync(SolicitudAmistad solicitud, CancellationToken cancellationToken = default)
        {
            _context.SolicitudesAmistad.Add(solicitud);
            await _context.SaveChangesAsync(cancellationToken);
            return solicitud;
        }

        public async Task<SolicitudAmistad> UpdateAsync(SolicitudAmistad solicitud, CancellationToken cancellationToken = default)
        {
            _context.SolicitudesAmistad.Update(solicitud);
            await _context.SaveChangesAsync(cancellationToken);
            return solicitud;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var s = await _context.SolicitudesAmistad.FindAsync(new object[] { id }, cancellationToken);
            if (s != null)
            {
                _context.SolicitudesAmistad.Remove(s);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        public async Task<SolicitudAmistad?> GetAmistadEntreUsuariosAsync(Guid usuarioAId, Guid usuarioBId, CancellationToken ct = default)
        {
            return await _context.SolicitudesAmistad
                .FirstOrDefaultAsync(s =>
                    s.Estado == EstadoSolicitud.Aceptada &&
                    ((s.RemitenteId == usuarioAId && s.DestinatarioId == usuarioBId) ||
                     (s.RemitenteId == usuarioBId && s.DestinatarioId == usuarioAId)),
                    ct);
        }

    }
}
