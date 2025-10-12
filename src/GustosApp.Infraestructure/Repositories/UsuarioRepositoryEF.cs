using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
             .Include(u => u.Restricciones)
               .Include(u => u.Gustos)
              .Include(u => u.CondicionesMedicas)
             .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid, ct);
        }

        public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
            => _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

        public Task<Usuario?> GetByUsernameAsync(string username, CancellationToken ct = default)
            => _db.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == username, ct);

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
    }
}
