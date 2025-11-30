using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.UseCases.AmistadUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace GustosApp.API.Hubs
{
    [Authorize]
    public class SolicitudesAmistadHub : Hub
    {
        private readonly AceptarSolicitudUseCase _aceptarSolicitud;
        private readonly RechazarSolicitudUseCase _rechazarSolicitud;
        private readonly ObtenerSolicitudesPendientesUseCase _obtenerPendientes;
        private readonly IMapper _mapper;
        private readonly IUsuarioRepository _usuarioRepository;
        public SolicitudesAmistadHub(
            AceptarSolicitudUseCase aceptarSolicitud,
            RechazarSolicitudUseCase rechazarSolicitud,
            ObtenerSolicitudesPendientesUseCase obtenerPendientes,
            IUsuarioRepository usuarioRepository,
            IMapper mapper)
        {
            _aceptarSolicitud = aceptarSolicitud;
            _rechazarSolicitud = rechazarSolicitud;
            _obtenerPendientes = obtenerPendientes;
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var uid = Context.User?.FindFirst("user_id")?.Value;
                if (string.IsNullOrEmpty(uid))
                {
                    Console.WriteLine("⚠️ Usuario conectado a SolicitudesAmistadHub sin user_id");
                    await base.OnConnectedAsync();
                    return;
                }

                var solicitudes = await _obtenerPendientes.HandleAsync(uid, CancellationToken.None);
                var solicitudesDTO = _mapper.Map<List<SolicitudAmistadResponse>>(solicitudes);

                await Clients.Caller.SendAsync("SolicitudesPendientes", solicitudesDTO);
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en OnConnectedAsync SolicitudesAmistadHub: {ex.Message}");
                await base.OnConnectedAsync();
            }
        }

        public async Task AceptarSolicitud(Guid solicitudId)
        {
            var uid = Context.User?.FindFirst("user_id")?.Value;
            if (uid == null) return;

            var solicitud = await _aceptarSolicitud.HandleAsync(uid, solicitudId, CancellationToken.None);

          
            var usuarioQueAcepto = await _usuarioRepository.GetByIdAsync(solicitud.DestinatarioId, CancellationToken.None);

         
            var emisor = await _usuarioRepository.GetByIdAsync(solicitud.RemitenteId, CancellationToken.None);

            if (emisor?.FirebaseUid != null && usuarioQueAcepto != null)
            {
               
                await Clients.User(emisor.FirebaseUid).SendAsync("SolicitudAceptada", new
                {
                    nombreUsuario = usuarioQueAcepto.Nombre,
                 
                });
            }
        }

        public async Task RechazarSolicitud(Guid solicitudId)
        {

            var uid = Context.User?.FindFirst("user_id")?.Value;

            await _rechazarSolicitud.HandleAsync(uid,solicitudId, CancellationToken.None);
        }
    }

}
