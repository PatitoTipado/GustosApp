using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IMiembroGrupoRepository
    {
        Task<MiembroGrupo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MiembroGrupo?> GetByGrupoYUsuarioAsync(Guid grupoId, string username, CancellationToken cancellationToken = default);
        Task<IEnumerable<MiembroGrupo>> GetMiembrosByGrupoIdAsync(Guid grupoId, CancellationToken cancellationToken = default);
        Task<IEnumerable<MiembroGrupo>> GetGruposByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<MiembroGrupo> CreateAsync(MiembroGrupo miembro, CancellationToken cancellationToken = default);
        Task<MiembroGrupo> UpdateAsync(MiembroGrupo miembro, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UsuarioEsMiembroActivoAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default);
        Task<int> ContarMiembrosActivosAsync(Guid grupoId, CancellationToken cancellationToken = default);
    }
}
