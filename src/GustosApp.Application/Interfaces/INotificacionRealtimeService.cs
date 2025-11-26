using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface INotificacionRealtimeService
    {
        Task EnviarNotificacionAsync(
          string usuarioDestinoId,
          string titulo,
          string mensaje,
          string tipo,
          CancellationToken ct,
            Guid? notificacionId = null,
          Guid? invitacionId = null );
    }
}
