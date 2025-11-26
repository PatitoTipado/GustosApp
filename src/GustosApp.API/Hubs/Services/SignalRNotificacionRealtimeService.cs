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

        public async Task EnviarNotificacionAsync(
    string firebaseUid,
    string titulo,
    string mensaje,
    string tipo,
    CancellationToken ct,
    Guid? notificacionId = null,
    Guid? invitacionId = null)
        {
            Console.WriteLine($"📡 Enviando notificación a usuario {firebaseUid} → {titulo}");

            await _hubContext.Clients.User(firebaseUid)
                .SendAsync("RecibirNotificacion", new
                {
                    Id = notificacionId,
                    InvitacionId = invitacionId,
                    Titulo = titulo,
                    Mensaje = mensaje,
                    Tipo = tipo,
                    FechaCreacion = DateTime.UtcNow
                }, ct);
        }
    }
    }
