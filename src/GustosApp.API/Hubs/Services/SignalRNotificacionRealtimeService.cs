using GustosApp.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GustosApp.API.Hubs.Services
{
    public class SignalRNotificacionRealtimeService : INotificacionRealtimeService
    {
        private readonly IHubContext<NotificacionesHub> _hubContext;

        public SignalRNotificacionRealtimeService(IHubContext<NotificacionesHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task EnviarNotificacionAsync(Guid usuarioDestinoId, string titulo, string mensaje, string tipo, CancellationToken ct)
        {
            await _hubContext.Clients.User(usuarioDestinoId.ToString())
                .SendAsync("RecibirNotificacion", new
                {
                    Titulo = titulo,
                    Mensaje = mensaje,
                    Tipo = tipo
                }, ct);
        }
    }
}
