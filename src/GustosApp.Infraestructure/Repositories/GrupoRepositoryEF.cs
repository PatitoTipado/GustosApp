using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class GrupoRepositoryEF : IGrupoRepository
    {
        private readonly GustosDbContext _context;

        public GrupoRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }

        public async Task<Grupo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Grupos
                .Include(g => g.Administrador)
                .Include(g => g.Miembros)
                    .ThenInclude(m => m.Usuario)
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<Grupo?> GetByCodigoInvitacionAsync(string codigo, CancellationToken cancellationToken = default)
        {
            return await _context.Grupos
                .Include(g => g.Administrador)
                .Include(g => g.Miembros)
                    .ThenInclude(m => m.Usuario)
                .FirstOrDefaultAsync(g => g.CodigoInvitacion == codigo && g.Activo, cancellationToken);
        }

        public async Task<IEnumerable<Grupo>> GetGruposByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.Grupos
                 .Include(g => g.Administrador)
                 .Include(g => g.Miembros).ThenInclude(m => m.Usuario)
                 .Where(g =>
                        g.AdministradorId == usuarioId ||
                        g.Miembros.Any(m => m.UsuarioId == usuarioId && m.Activo)
                        )
                        .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Grupo>> GetGruposAdministradosByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.Grupos
                .Include(g => g.Administrador)
                .Include(g => g.Miembros)
                    .ThenInclude(m => m.Usuario)
                .Where(g => g.AdministradorId == usuarioId && g.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<Grupo> CreateAsync(Grupo grupo, CancellationToken cancellationToken = default)
        {
            _context.Grupos.Add(grupo);
            await _context.SaveChangesAsync(cancellationToken);
            return grupo;
        }

        public async Task<Grupo> UpdateAsync(Grupo grupo, CancellationToken cancellationToken = default)
        {
            _context.Grupos.Update(grupo);
            await _context.SaveChangesAsync(cancellationToken);
            return grupo;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var grupo = await _context.Grupos.FindAsync(id);
            if (grupo != null)
            {
                _context.Grupos.Remove(grupo);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Grupos.AnyAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<bool> UsuarioEsMiembroAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.MiembrosGrupos
                .AnyAsync(m => m.GrupoId == grupoId && m.UsuarioId == usuarioId, cancellationToken);
        }

        public async Task<bool> UsuarioEsAdministradorAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default)
        {
            // Check directly in the Grupos table if the user is the administrator
            var grupo = await _context.Grupos
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == grupoId, cancellationToken);
            
            return grupo != null && grupo.AdministradorId == usuarioId;
        }

    }
}
