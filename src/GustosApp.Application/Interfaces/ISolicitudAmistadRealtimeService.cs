using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Interfaces
{
    public interface ISolicitudAmistadRealtimeService
    {
        Task EnviarSolicitudAsync(
            string destinatarioUid,
            SolicitudAmistad solicitud,
            CancellationToken ct);
    }

}
