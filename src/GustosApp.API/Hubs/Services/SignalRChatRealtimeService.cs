using GustosApp.API.Hubs.GustosApp.API.Hubs;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.SignalR;

namespace GustosApp.API.Hubs.Services
{
    public class SignalRChatRealtimeService : IChatRealTimeService
    {

        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRChatRealtimeService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task NotificarGrupoChat(Guid grupoId, Guid usuarioId, string nombre)
        {
            await _hubContext.Clients.Group(grupoId.ToString())
                .SendAsync("GroupMemberJoined", new
                {
                    GroupId = grupoId,
                    UserId = usuarioId,
                    UserName = nombre
                });

            // También mensaje de sistema al chat
            await _hubContext.Clients.Group(grupoId.ToString())
             .SendAsync("ReceiveMessage", new
             {
                 usuario = "Sistema",
                 mensaje = $"{nombre} se unió al grupo 👋",
                 fecha = DateTime.UtcNow
             });
        }



    }
}
