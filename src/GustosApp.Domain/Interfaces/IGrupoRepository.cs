using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IGrupoRepository
    {
        Task<Grupo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Grupo?> GetByCodigoInvitacionAsync(string codigo, CancellationToken cancellationToken = default);
        Task<IEnumerable<Grupo>> GetGruposByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Grupo>> GetGruposAdministradosByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<Grupo> CreateAsync(Grupo grupo, CancellationToken cancellationToken = default);
        Task<Grupo> UpdateAsync(Grupo grupo, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UsuarioEsMiembroAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default);
        Task<bool> UsuarioEsAdministradorAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default);
    }
}
