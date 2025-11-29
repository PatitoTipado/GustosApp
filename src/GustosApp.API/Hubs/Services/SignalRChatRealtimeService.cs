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

        public async Task UsuarioSeUnio(Guid grupoId, Guid usuarioId, string nombre)
        {
            await _hubContext.Clients.Group(grupoId.ToString())
                .SendAsync("UsuarioSeUnio", new
                {
                    usuarioId,
                    nombre
                });


        }

        public async Task UsuarioExpulsadoDelGrupo(Guid grupoId, string firebaseUid, string nombreGrupo)
        {
            
            await _hubContext.Clients.User(firebaseUid).SendAsync("KickedFromGroup", new
            {
                grupoId,
                nombreGrupo
            });
        }

        public async Task UsuarioAbandono(Guid grupoId, Guid usuarioId, string nombre, string firebaseUid)
        {
       
            await _hubContext.Clients.Group(grupoId.ToString())
                .SendAsync("UsuarioAbandonoGrupo", new
                {
                    usuarioId,
                    nombre,
                    firebaseUid
                });

            await _hubContext.Groups.RemoveFromGroupAsync(
                firebaseUid,           
                grupoId.ToString()
            );


        }

    }
}
