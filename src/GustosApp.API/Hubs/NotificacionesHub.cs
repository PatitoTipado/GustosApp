using System.Text.RegularExpressions;
using GustosApp.Application.Tests.mocks;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.SignalR;

namespace GustosApp.API.Hubs
{
    public class NotificacionesHub : Hub
    {


        public class NotificacionesHub : Hub
        {
            private readonly ObtenerNotificacionesUsuarioUseCase _obtenerNotificaciones;
            private readonly MarcarNotificacionLeidaUseCase _marcarLeida;
            private readonly EliminarNotificacionUseCase _eliminarNotificacion;

            public NotificacionesHub(
                ObtenerNotificacionesUsuarioUseCase obtenerNotificaciones,
                MarcarNotificacionLeidaUseCase marcarLeida,
                EliminarNotificacionUseCase eliminarNotificacion)
            {
                _obtenerNotificaciones = obtenerNotificaciones;
                _marcarLeida = marcarLeida;
                _eliminarNotificacion = eliminarNotificacion;
            }

            // 🔹 Se ejecuta al conectarse un usuario
            public override async Task OnConnectedAsync()
            {
                var uid = Context.User?.FindFirst("user_id")?.Value;
                if (uid == null) return;

                await base.OnConnectedAsync();

                if (Guid.TryParse(userId, out var usuarioGuid))
                {
                    var notificaciones = await _obtenerNotificaciones.HandleAsync(usuarioGuid, CancellationToken.None);

               
                    await Clients.Caller.SendAsync("CargarNotificaciones", notificaciones);
                }

                await base.OnConnectedAsync();
            }

           
            public async Task EnviarNotificacionATodos(string mensaje)
            {
                await Clients.All.SendAsync("RecibirNotificacion", new { Mensaje = mensaje });
            }

            public async Task EnviarNotificacionAUsuario(string usuarioId, string mensaje)
            {
                await Clients.User(usuarioId).SendAsync("RecibirNotificacion", new { Mensaje = mensaje });
            }

            // 🔹 Cliente marca una notificación como leída
            public async Task MarcarComoLeida(Guid notificacionId)
            {
                await _marcarLeida.HandleAsync(notificacionId, CancellationToken.None);
                await Clients.Caller.SendAsync("NotificacionMarcadaLeida", notificacionId);
            }

            // 🔹 Cliente elimina una notificación (p. ej. si acepta o rechaza invitación)
            public async Task EliminarNotificacion(Guid notificacionId)
            {
                await _eliminarNotificacion.HandleAsync(notificacionId, CancellationToken.None);
                await Clients.Caller.SendAsync("NotificacionEliminada", notificacionId);
            }
        }
    }
