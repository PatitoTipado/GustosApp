using GustosApp.Domain.Common;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GustosApp.Infraestructure.Repositories
{
    public class MiembroGrupoRepositoryEF : IMiembroGrupoRepository
    {
        private readonly GustosDbContext _context;

        public MiembroGrupoRepositoryEF(GustosDbContext context)
        {
            _context = context;
        }

        public async Task<MiembroGrupo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.MiembrosGrupos
                .Include(m => m.Grupo)
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<MiembroGrupo?> GetByGrupoYUsuarioAsync(Guid grupoId, string username, CancellationToken cancellationToken = default)
        {
            return await _context.MiembrosGrupos
                 .Include(m => m.Grupo)
                 .Include(m => m.Usuario)
                 .FirstOrDefaultAsync(
            m => m.GrupoId == grupoId && m.Usuario.IdUsuario == username,
            cancellationToken
        );
        }

        public async Task<IEnumerable<MiembroGrupo>> GetMiembrosByGrupoIdAsync(Guid grupoId, CancellationToken cancellationToken = default)
        {
            return await _context.MiembrosGrupos
                .Include(m => m.Usuario)
                .Where(m => m.GrupoId == grupoId && m.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<MiembroGrupo>> GetGruposByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.MiembrosGrupos
                .Include(m => m.Grupo)
                .Where(m => m.UsuarioId == usuarioId && m.Activo)
                .ToListAsync(cancellationToken);
        }

        public async Task<MiembroGrupo> CreateAsync(MiembroGrupo miembro, CancellationToken cancellationToken = default)
        {
            _context.MiembrosGrupos.Add(miembro);
            await _context.SaveChangesAsync(cancellationToken);
            return miembro;
        }

        public async Task<MiembroGrupo> UpdateAsync(MiembroGrupo miembro, CancellationToken cancellationToken = default)
        {
            _context.MiembrosGrupos.Update(miembro);
            await _context.SaveChangesAsync(cancellationToken);
            return miembro;
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var miembro = await _context.MiembrosGrupos.FindAsync(id);
            if (miembro != null)
            {
                _context.MiembrosGrupos.Remove(miembro);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.MiembrosGrupos.AnyAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<bool> UsuarioEsMiembroActivoAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.MiembrosGrupos
                .AnyAsync(m => m.GrupoId == grupoId && m.UsuarioId == usuarioId && m.Activo, cancellationToken);
        }

        public async Task<int> ContarMiembrosActivosAsync(Guid grupoId, CancellationToken cancellationToken = default)
        {
            return await _context.MiembrosGrupos
                .CountAsync(m => m.GrupoId == grupoId && m.Activo, cancellationToken);
        }

        public async Task<bool> DesactivarMiembroDeGrupo(Guid idGrupo, Guid idUsuario)
        {
            // Obtener miembro
            MiembroGrupo? miembro = _context.MiembrosGrupos
                .FirstOrDefault(m => m.GrupoId == idGrupo && m.UsuarioId == idUsuario && m.Activo);

            if (miembro == null)
            {
                throw new UnauthorizedAccessException("El usuario no está activo en el grupo o no existe");
            }

            miembro.afectarRecomendacion = false;

            await UpdateAsync(miembro);

            return true;
        }

        public async Task<bool> ActivarMiembro(Guid idGrupo, Guid idUsuario)
        {
            MiembroGrupo? miembro = _context.MiembrosGrupos
                .FirstOrDefault(m => m.GrupoId == idGrupo && m.UsuarioId == idUsuario && m.Activo);

            if (miembro == null)
            {
                throw new UnauthorizedAccessException("El usuario no está activo en el grupo o no existe");
            }

            miembro.afectarRecomendacion = true;

            await UpdateAsync(miembro);

            return true;
        }

        public async Task<UsuarioPreferencias> obtenerMiembrosActivosConSusPreferenciasYCondiciones(Guid grupoId)
        {

            var miembros = await _context.MiembrosGrupos
                .Include(m => m.Usuario)
                    .ThenInclude(u => u.Restricciones)
                .Include(m => m.Usuario)
                    .ThenInclude(u => u.CondicionesMedicas)
                        .Where(m => m.GrupoId == grupoId && m.afectarRecomendacion && m.Activo)
                .ToListAsync();

            var todasLasPreferencias = miembros
                .SelectMany(m => m.Usuario.Restricciones)
                .Select(p => p.Nombre)
                .Distinct()
                .ToList();

            var todasLasCondiciones = miembros
                .SelectMany(m => m.Usuario.CondicionesMedicas)
                .Select(c => c.Nombre)
                .Distinct()
                .ToList();

            return new UsuarioPreferencias() {Restricciones=todasLasPreferencias,CondicionesMedicas = todasLasCondiciones };


        }
    }
}
