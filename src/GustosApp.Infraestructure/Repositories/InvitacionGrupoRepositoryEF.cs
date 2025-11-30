using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class InvitacionGrupoRepositoryEF : IInvitacionGrupoRepository
    {
        private readonly GustosDbContext _context;

        public InvitacionGrupoRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }

        public async Task<InvitacionGrupo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.InvitacionesGrupos
                .Include(i => i.Grupo)
                .Include(i => i.UsuarioInvitado)
                .Include(i => i.UsuarioInvitador)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<InvitacionGrupo?> GetInvitacionPendienteAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.InvitacionesGrupos
                .Include(i => i.Grupo)
                .Include(i => i.UsuarioInvitado)
                .Include(i => i.UsuarioInvitador)
                .FirstOrDefaultAsync(i => i.GrupoId == grupoId && i.UsuarioInvitadoId == usuarioId && i.Estado == EstadoInvitacion.Pendiente, cancellationToken);
        }

        public async Task<IEnumerable<InvitacionGrupo>> GetInvitacionesByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.InvitacionesGrupos
                .Include(i => i.Grupo)
                .Include(i => i.UsuarioInvitado)
                .Include(i => i.UsuarioInvitador)
                .Where(i => i.UsuarioInvitadoId == usuarioId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<InvitacionGrupo>> GetInvitacionesByGrupoIdAsync(Guid grupoId, CancellationToken cancellationToken = default)
        {
            return await _context.InvitacionesGrupos
                .Include(i => i.Grupo)
                .Include(i => i.UsuarioInvitado)
                .Include(i => i.UsuarioInvitador)
                .Where(i => i.GrupoId == grupoId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<InvitacionGrupo>> GetInvitacionesPendientesByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.InvitacionesGrupos
                .Include(i => i.Grupo)
                .Include(i => i.UsuarioInvitado)
                .Include(i => i.UsuarioInvitador)
                .Where(i => i.UsuarioInvitadoId == usuarioId && i.Estado == EstadoInvitacion.Pendiente)
                .ToListAsync(cancellationToken);
        }

        public async Task<InvitacionGrupo> CreateAsync(InvitacionGrupo invitacion, CancellationToken cancellationToken = default)
        {
            _context.InvitacionesGrupos.Add(invitacion);
            await _context.SaveChangesAsync(cancellationToken);
            return invitacion;
        }

        public async Task<InvitacionGrupo> UpdateAsync(InvitacionGrupo invitacion, CancellationToken cancellationToken = default)
        {
            _context.InvitacionesGrupos.Update(invitacion);
            await _context.SaveChangesAsync(cancellationToken);
            return invitacion;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var invitacion = await _context.InvitacionesGrupos.FindAsync(id);
            if (invitacion != null)
            {
                _context.InvitacionesGrupos.Remove(invitacion);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.InvitacionesGrupos.AnyAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<bool> ExisteInvitacionPendienteAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.InvitacionesGrupos
                .AnyAsync(i => i.GrupoId == grupoId && i.UsuarioInvitadoId == usuarioId && i.Estado == EstadoInvitacion.Pendiente, cancellationToken);
        }
        public async Task<InvitacionGrupo?> ObtenerUltimaInvitacionAsync(Guid grupoId, Guid usuarioInvitadoId, CancellationToken ct)
        {
            return await _context.InvitacionesGrupos
                .Where(i => i.GrupoId == grupoId && i.UsuarioInvitadoId == usuarioInvitadoId)
                .OrderByDescending(i => i.FechaInvitacion)
                .FirstOrDefaultAsync(ct);
        }

        public async Task MarcarInvitacionesExpiradasAsync(CancellationToken cancellationToken = default)
        {
            var invitacionesExpiradas = await _context.InvitacionesGrupos
                .Where(i => i.Estado == EstadoInvitacion.Pendiente && i.FechaExpiracion < DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var invitacion in invitacionesExpiradas)
            {
                invitacion.MarcarComoExpirada();
            }

            if (invitacionesExpiradas.Any())
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
