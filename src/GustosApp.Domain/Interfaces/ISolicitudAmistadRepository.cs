using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Domain.Interfaces
{
    public interface ISolicitudAmistadRepository
    {
        Task<SolicitudAmistad?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<SolicitudAmistad>> GetSolicitudesPendientesByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SolicitudAmistad>> GetSolicitudesEnviadasByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Usuario>> GetAmigosByUsuarioIdAsync(Guid usuarioId, CancellationToken cancellationToken = default);
        Task<bool> ExisteSolicitudPendienteAsync(Guid remitenteId, Guid destinatarioId, CancellationToken cancellationToken = default);
        Task<SolicitudAmistad> CreateAsync(SolicitudAmistad solicitud, CancellationToken cancellationToken = default);
        Task<SolicitudAmistad> UpdateAsync(SolicitudAmistad solicitud, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
