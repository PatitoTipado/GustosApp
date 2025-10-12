﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByFirebaseUidAsync(string firebaseUid, CancellationToken ct = default);
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<Usuario?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Usuario usuario, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<IEnumerable<Usuario>> GetAllAsync(int limit = 100, CancellationToken ct = default);
    }
}
