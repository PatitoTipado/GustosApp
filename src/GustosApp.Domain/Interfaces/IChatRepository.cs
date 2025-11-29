using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IChatRepository
    {
        Task<IEnumerable<ChatMensaje>> GetMessagesByGrupoIdAsync(Guid grupoId, CancellationToken cancellationToken = default);
        Task<ChatMensaje> AddMessageAsync(ChatMensaje message, CancellationToken cancellationToken = default);

        Task<ChatMensaje> AddSystemMessageAsync(ChatMensaje mensaje, CancellationToken ct);

    }
}
