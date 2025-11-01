using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class UsuarioRepositoryEF : IUsuarioRepository
    {
        private readonly GustosDbContext _db;

        public UsuarioRepositoryEF(GustosDbContext db) => _db = db;

        public async Task<Usuario?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken ct = default)
        {
            return await _db.Usuarios
         .Include(u => u.Gustos)
             .ThenInclude(g => g.Tags)
         .Include(u => u.Restricciones)
             .ThenInclude(r => r.TagsProhibidos)
         .Include(u => u.CondicionesMedicas)
             .ThenInclude(c => c.TagsCriticos)
         .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid, ct);
        }

        public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
            => _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<Usuario?> GetByUsernameAsync(string username, CancellationToken ct = default)
            => _db.Usuarios
                .Include(u => u.Gustos)
                .Include(u => u.Visitados)
                .FirstOrDefaultAsync(u => u.IdUsuario == username, ct);

        public async Task<IEnumerable<Usuario>> GetAllAsync(int limit = 100, CancellationToken ct = default)
        {
            return await _db.Usuarios.Take(limit).ToListAsync(ct);
        }

        public Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.Usuarios.FirstOrDefaultAsync(u => u.Id == id, ct);

        public async Task AddAsync(Usuario usuario, CancellationToken ct = default)
            => await _db.Usuarios.AddAsync(usuario, ct);

        public Task SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public Task<Usuario?> GetByFirebaseUidWithGustosAsync(string firebaseUid, CancellationToken ct = default)
        => _db.Usuarios
              .Include(u => u.Gustos)
              .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid, ct);

        public async Task<List<Usuario>> GetAllWithGustosAsync(CancellationToken ct = default)
         => await _db.Usuarios
                .Include(u => u.Gustos)
                .ToListAsync(ct);

        public async Task<Usuario> GetUsuarioConRestriccionesAsync(Guid usuarioId)
        {
            return await _db.Usuarios
                .Include(u => u.Restricciones) 
                .FirstOrDefaultAsync(u => u.Id == usuarioId);
        }

        public async Task<IEnumerable<Usuario>> GetAllExceptAsync(Guid excludeId, int limit, CancellationToken ct)
        {
            return await _db.Usuarios
                .Where(u => u.Id != excludeId && u.Activo)
                .OrderBy(u => u.Nombre)
                .Take(limit)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Usuario>> BuscarPorUsernameAsync(string search, Guid excludeId, CancellationToken ct)
        {
            var normalized = search.Trim().ToLower();

            return await _db.Usuarios
                .Where(u => u.Id != excludeId &&
                            u.Activo &&
                            u.IdUsuario.ToLower().Contains(normalized))
                .OrderBy(u => u.IdUsuario)
                .Take(50)
                .ToListAsync(ct);
        }

    }
}
