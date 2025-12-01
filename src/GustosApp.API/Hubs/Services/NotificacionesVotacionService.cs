using GustosApp.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace GustosApp.API.Hubs.Services
{
    public class NotificacionesVotacionService : INotificacionesVotacionService
    {
        private readonly IHubContext<VotacionesHub> _hub;

        public NotificacionesVotacionService(IHubContext<VotacionesHub> hub)
        {
            _hub = hub;
        }

        public Task NotificarVotoRegistrado(Guid grupoId, Guid votacionId)
        {
            return _hub.Clients.Group(grupoId.ToString())
                .SendAsync("VotoRegistrado", new { votacionId });
        }

        public Task NotificarResultadosActualizados(Guid grupoId, Guid votacionId)
        {
            return _hub.Clients.Group(grupoId.ToString())
                .SendAsync("ResultadosActualizados", new { votacionId });
        }

        public Task NotificarEmpate(Guid grupoId, Guid votacionId)
        {
            return _hub.Clients.Group(grupoId.ToString())
                .SendAsync("EmpateDetectado", new { votacionId });
        }

        public Task NotificarGanador(Guid grupoId, Guid votacionId, Guid restauranteGanadorId)
        {
            return _hub.Clients.Group(grupoId.ToString())
                .SendAsync("GanadorSeleccionado", new { votacionId, restauranteGanadorId });
        }

        public Task NotificarVotacionCerrada(Guid grupoId, Guid votacionId, Guid? restauranteGanadorId)
        {
            return _hub.Clients.Group(grupoId.ToString())
                .SendAsync("VotacionCerrada", new { votacionId, restauranteGanadorId });
        }
    }

}