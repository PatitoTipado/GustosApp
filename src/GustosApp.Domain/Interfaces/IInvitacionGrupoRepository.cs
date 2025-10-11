using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IInvitacionGrupoRepository
    {
        Task<InvitacionGrupo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<InvitacionGrupo?> GetInvitacionPendienteAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InvitacionGrupo>> GetInvitacionesByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InvitacionGrupo>> GetInvitacionesByGrupoIdAsync(Guid grupoId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InvitacionGrupo>> GetInvitacionesPendientesByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<InvitacionGrupo> CreateAsync(InvitacionGrupo invitacion, CancellationToken cancellationToken = default);
        Task<InvitacionGrupo> UpdateAsync(InvitacionGrupo invitacion, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExisteInvitacionPendienteAsync(Guid grupoId, Guid usuarioId, CancellationToken cancellationToken = default);
        Task MarcarInvitacionesExpiradasAsync(CancellationToken cancellationToken = default);
    }
}
