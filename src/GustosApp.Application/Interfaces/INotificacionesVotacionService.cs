using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface INotificacionesVotacionService
    {
        Task NotificarVotoRegistrado(Guid grupoId, Guid votacionId);
        Task NotificarResultadosActualizados(Guid grupoId, Guid votacionId);
        Task NotificarEmpate(Guid grupoId, Guid votacionId);
        Task NotificarGanador(Guid grupoId, Guid votacionId, Guid restauranteGanadorId);
        Task NotificarVotacionCerrada(Guid grupoId, Guid votacionId, Guid? restauranteGanadorId);
    }

}
