using System.Security.Claims;
using System.Text.RegularExpressions;
using AutoMapper;
using Azure.Core;
using GustosApp.API.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.GrupoUseCases.InvitacionGrupoUseCases;
using GustosApp.Application.UseCases.NotificacionUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure.Services;
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
        private readonly IUsuariosActivosService _usuariosActivos;


        public NotificacionesHub(
                ObtenerNotificacionesUsuarioUseCase obtenerNotificaciones,
                MarcarNotificacionLeidaUseCase marcarLeida,
                EliminarNotificacionUseCase eliminarNotificacion,
                ObtenerUsuarioUseCase obtenerUsuario,
                AceptarInvitacionGrupoUseCase aceptar,
                ObtenerNotificacionUsuarioUseCase obtenerNotificacion,
                IMapper mapper, IUsuariosActivosService usuariosActivos)
        {
            _obtenerNotificaciones = obtenerNotificaciones;
            _marcarLeida = marcarLeida;
            _eliminarNotificacion = eliminarNotificacion;
            _obtenerUsuario = obtenerUsuario;
            _aceptarInvitacionGrupo = aceptar;
            _obtenerNotificacion = obtenerNotificacion;
            _mapper = mapper;
            _usuariosActivos = usuariosActivos;
        }

        // Se ejecuta al conectarse un usuario
        public override async Task OnConnectedAsync()
        {
            try
            {
                var firebaseUid = Context.User?.FindFirst("user_id")?.Value
                 ?? Context.User?.FindFirst("firebase_uid")?.Value
                ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(firebaseUid))
                {
                    Console.WriteLine("⚠️ Usuario conectado sin firebaseUid");
                    await base.OnConnectedAsync();
                    return;
                }

                var usuario = await _obtenerUsuario.HandleAsync(FirebaseUid: firebaseUid, ct: CancellationToken.None);
                if (usuario == null)
                {
                    Console.WriteLine($"⚠️ No se encontró usuario con firebaseUid: {firebaseUid}");
                    await base.OnConnectedAsync();
                    return;
                }

                _usuariosActivos.UsuarioConectado(firebaseUid);

                var notificaciones = await _obtenerNotificaciones.HandleAsync(usuario.Id, CancellationToken.None);

                //DTO
                var notificacionesDTO = _mapper.Map<List<NotificacionDTO>>(notificaciones);

                await Clients.Caller.SendAsync("CargarNotificaciones", notificacionesDTO);
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en OnConnectedAsync NotificacionesHub: {ex.Message}");
                await base.OnConnectedAsync();
            }
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
        public override Task OnDisconnectedAsync(Exception? ex)
        {
            var uid = Context.User?.FindFirst("user_id")?.Value;
            if (uid != null)
            {
                _usuariosActivos.UsuarioDesconectado(uid);
            }
            return base.OnDisconnectedAsync(ex);
        }
    }
}
