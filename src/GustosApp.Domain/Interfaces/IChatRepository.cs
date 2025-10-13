using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface IChatRepository
    {
        Task<IEnumerable<ChatMessage>> GetMessagesByGrupoIdAsync(Guid grupoId, CancellationToken cancellationToken = default);
        Task<ChatMessage> AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default);
    }
}
