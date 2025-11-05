using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.SignalR;

namespace GustosApp.API.Hubs.Services
{
    public class SignalRSolicitudAmistadRealtimeService : ISolicitudAmistadRealtimeService
    {
        private readonly IHubContext<SolicitudesAmistadHub> _hubContext;
        private readonly IMapper _mapper;

        public SignalRSolicitudAmistadRealtimeService(IHubContext<SolicitudesAmistadHub> hubContext
            , IMapper mapper)
        {
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task EnviarSolicitudAsync(
            string destinatarioUid,
           SolicitudAmistad solicitud,
            CancellationToken ct)
        {
            Console.WriteLine($" Enviando solicitud de amistad a {destinatarioUid}");

            var dto = _mapper.Map<SolicitudAmistadResponse>(solicitud);

            await _hubContext.Clients.User(destinatarioUid)
                .SendAsync("RecibirSolicitudAmistad", dto, ct);
        }
    }

}
