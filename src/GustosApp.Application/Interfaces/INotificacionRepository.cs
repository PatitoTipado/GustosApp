using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.Interfaces
{
    public interface INotificacionRepository
    {
        Task crearAsync(Notificacion notificacion,CancellationToken cancellationToken);
        Task EliminarAsync(Guid notificacionId, CancellationToken ct);
        Task MarcarComoLeidaAsync(Guid notificacionId, CancellationToken ct);
        Task<List<Notificacion>> ObtenerNotificacionPorUsuarioAsync(Guid usuarioId, CancellationToken cancellationToken);
    }
}
