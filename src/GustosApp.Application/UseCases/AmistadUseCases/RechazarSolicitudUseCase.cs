using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.AmistadUseCases
{
    public class RechazarSolicitudUseCase
    {
        private readonly ISolicitudAmistadRepository _solicitudRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public RechazarSolicitudUseCase(ISolicitudAmistadRepository solicitudRepository, IUsuarioRepository usuarioRepository)
        {
            _solicitudRepository = solicitudRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<SolicitudAmistad> HandleAsync(string firebaseUid, Guid solicitudId, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null) throw new UnauthorizedAccessException("Usuario no encontrado");

            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId, cancellationToken);
            if (solicitud == null) throw new ArgumentException("Solicitud no encontrada");

            if (solicitud.DestinatarioId != usuario.Id) throw new UnauthorizedAccessException("Solo el destinatario puede rechazar la solicitud");

            solicitud.Rechazar();
            await _solicitudRepository.UpdateAsync(solicitud, cancellationToken);

            return solicitud;
        }
    }
}
