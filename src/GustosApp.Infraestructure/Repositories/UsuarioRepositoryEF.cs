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
    }
}
