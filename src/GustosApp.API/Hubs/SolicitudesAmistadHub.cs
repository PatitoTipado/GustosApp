using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.UseCases;
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

        public SolicitudesAmistadHub(
            AceptarSolicitudUseCase aceptarSolicitud,
            RechazarSolicitudUseCase rechazarSolicitud,
            ObtenerSolicitudesPendientesUseCase obtenerPendientes,
            IMapper mapper)
        {
            _aceptarSolicitud = aceptarSolicitud;
            _rechazarSolicitud = rechazarSolicitud;
            _obtenerPendientes = obtenerPendientes;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var uid = Context.User?.FindFirst("user_id")?.Value;
            if (uid == null) return;

            var solicitudes = await _obtenerPendientes.HandleAsync(uid, CancellationToken.None);
            var solicitudesDTO = _mapper.Map<List<SolicitudAmistadResponse>>(solicitudes);


            await Clients.Caller.SendAsync("SolicitudesPendientes", solicitudesDTO);
        }

        public async Task AceptarSolicitud(Guid solicitudId)
        {
            var uid = Context.User?.FindFirst("user_id")?.Value;
            if (uid == null) return;

            await _aceptarSolicitud.HandleAsync(uid, solicitudId, CancellationToken.None);

            // Podés notificar al remitente si querés
            // await Clients.User(remitenteUid).SendAsync("SolicitudAceptada", solicitudId);
        }

        public async Task RechazarSolicitud(Guid solicitudId)
        {

            var uid = Context.User?.FindFirst("user_id")?.Value;

            await _rechazarSolicitud.HandleAsync(uid,solicitudId, CancellationToken.None);
        }
    }

}
