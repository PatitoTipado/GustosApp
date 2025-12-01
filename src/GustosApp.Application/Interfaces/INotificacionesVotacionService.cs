using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Common;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Interfaces
{
    public interface INotificacionesVotacionService
    {
        Task NotificarVotoRegistrado(Guid grupoId, EventoVotoRegistrado evento);
        Task NotificarResultadosActualizados(Guid grupoId, Guid votacionId);
        Task NotificarEmpate(Guid grupoId, Guid votacionId);
        Task NotificarGanador(Guid grupoId, Guid votacionId, Guid restauranteGanadorId);
        Task NotificarVotacionCerrada(Guid grupoId, Guid votacionId, Guid? restauranteGanadorId);

        Task NotificarVotacionIniciada(VotacionGrupo votacion);
    }

}
