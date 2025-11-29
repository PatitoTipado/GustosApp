using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface IChatRealTimeService
    {
      

        Task NotificarGrupoChat(Guid grupoId, Guid usuarioId, string nombre);
    }

}
