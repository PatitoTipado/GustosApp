using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.Infraestructure.Repositories
{
    public class ChatRepositoryEF : IChatRepository
    {
        private readonly GustosDbContext _context;

        public ChatRepositoryEF(GustosDbContext context) => _context = context;

        public async Task<IEnumerable<ChatMessage>> GetMessagesByGrupoIdAsync(Guid grupoId, CancellationToken cancellationToken = default)
        {
            return await _context.ChatMessages
            .Where(m => m.GrupoId == grupoId)
                .OrderByDescending(m => m.FechaEnvio)
                 .Take(50)
                 .OrderBy(m => m.FechaEnvio)
                 .ToListAsync(cancellationToken);

        }

        public async Task<ChatMessage> AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
        {
            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync(cancellationToken);
            return message;
        }
    }
}
