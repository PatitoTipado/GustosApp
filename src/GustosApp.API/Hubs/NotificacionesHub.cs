using Microsoft.AspNetCore.SignalR;

namespace GustosApp.API.Hubs
{
    public class NotificacionesHub : Hub
    {
        public async Task EnviarNotificacionATodos(string mensaje)
        {
            await Clients.All.SendAsync("RecibirNotificacion", mensaje);
        }

        public async Task EnviarNotificacionAUsuario(string usuarioId, string mensaje)
        {
            await Clients.User(usuarioId).SendAsync("RecibirNotificacion", mensaje);
        }
    }
}
