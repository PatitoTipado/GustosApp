using System.Text.RegularExpressions;
using AutoMapper;
using Azure.Core;
using GustosApp.API.DTO;
using GustosApp.Application.Tests.mocks;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.SignalR;

namespace GustosApp.API.Hubs
{

    public class NotificacionesHub : Hub
    {
        private readonly ObtenerNotificacionesUsuarioUseCase _obtenerNotificaciones;
        private readonly MarcarNotificacionLeidaUseCase _marcarLeida;
        private readonly EliminarNotificacionUseCase _eliminarNotificacion;
        private readonly ObtenerUsuarioUseCase _obtenerUsuario;
        private readonly AceptarInvitacionGrupoUseCase _aceptarInvitacionGrupo;
        private readonly ObtenerNotificacionUsuarioUseCase _obtenerNotificacion;
        private readonly IMapper _mapper;

        public NotificacionesHub(
                ObtenerNotificacionesUsuarioUseCase obtenerNotificaciones,
                MarcarNotificacionLeidaUseCase marcarLeida,
                EliminarNotificacionUseCase eliminarNotificacion,
                ObtenerUsuarioUseCase obtenerUsuario,
                AceptarInvitacionGrupoUseCase aceptar,
                ObtenerNotificacionUsuarioUseCase obtenerNotificacion,
                IMapper mapper)
        {
            _obtenerNotificaciones = obtenerNotificaciones;
            _marcarLeida = marcarLeida;
            _eliminarNotificacion = eliminarNotificacion;
            _obtenerUsuario = obtenerUsuario;
            _aceptarInvitacionGrupo = aceptar;
            _obtenerNotificacion = obtenerNotificacion;
            _mapper = mapper;
        }

        // Se ejecuta al conectarse un usuario
        public override async Task OnConnectedAsync()
        {

            var firebaseUid = Context.User?.FindFirst("user_id")?.Value;
            if (firebaseUid == null) return;

            var usuario = await _obtenerUsuario.HandleAsync(FirebaseUid :firebaseUid, ct:CancellationToken.None);
            if (usuario == null) return;

            var notificaciones = await _obtenerNotificaciones.HandleAsync(usuario.Id, CancellationToken.None);

            //DTO
            var notificacionesDTO = _mapper.Map<List<NotificacionDTO>>(notificaciones);


            await Clients.Caller.SendAsync("CargarNotificaciones", notificacionesDTO);
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


        public async Task MarcarComoLeida(Guid notificacionId)
        {
            await _marcarLeida.HandleAsync(notificacionId, CancellationToken.None);
            await Clients.Caller.SendAsync("NotificacionMarcadaLeida", notificacionId);
        }


        public async Task RechazarInvitacion(Guid notificacionId)
        {
            await _eliminarNotificacion.HandleAsync(notificacionId, CancellationToken.None);
            await Clients.Caller.SendAsync("NotificacionEliminada", notificacionId);
        }

        public async Task AceptarInvitacion(Guid notificacionId)
        {
            var uid = Context.User?.FindFirst("user_id")?.Value;
            if (uid == null) return;

            var usuario = await _obtenerUsuario.HandleAsync(FirebaseUid : uid, ct:CancellationToken.None);
            if (usuario == null) return;

            
            var notificacion = await _obtenerNotificacion.HandleAsync(notificacionId, CancellationToken.None);
            if (notificacion == null)
            {
                Console.WriteLine($"⚠️ No se encontró la notificación con ID {notificacionId}");
                return;
            }

            
            if (!notificacion.InvitacionId.HasValue)
            {
                Console.WriteLine($"⚠️ La notificación {notificacionId} no tiene un InvitacionId asociado.");
                return;
            }

           
            await _aceptarInvitacionGrupo.HandleAsync(usuario.FirebaseUid, notificacion.InvitacionId.Value, CancellationToken.None);

            
            await _eliminarNotificacion.HandleAsync(notificacionId, CancellationToken.None);

           
            await Clients.Caller.SendAsync("NotificacionEliminada", notificacionId);
        }
    }
}
