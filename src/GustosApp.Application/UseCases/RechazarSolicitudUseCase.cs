
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
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

        public async Task<SolicitudAmistadResponse> HandleAsync(string firebaseUid, Guid solicitudId, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null) throw new UnauthorizedAccessException("Usuario no encontrado");

            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId, cancellationToken);
            if (solicitud == null) throw new ArgumentException("Solicitud no encontrada");

            if (solicitud.DestinatarioId != usuario.Id) throw new UnauthorizedAccessException("Solo el destinatario puede rechazar la solicitud");

            solicitud.Rechazar();
            await _solicitudRepository.UpdateAsync(solicitud, cancellationToken);

            return new SolicitudAmistadResponse
            {
                Id = solicitud.Id,
                Remitente = new UsuarioSimpleResponse { Id = solicitud.Remitente.Id, Nombre = solicitud.Remitente.Nombre + " " + solicitud.Remitente.Apellido, Email = solicitud.Remitente.Email, FotoPerfilUrl = solicitud.Remitente.FotoPerfilUrl },
                Destinatario = new UsuarioSimpleResponse { Id = solicitud.Destinatario.Id, Nombre = solicitud.Destinatario.Nombre + " " + solicitud.Destinatario.Apellido, Email = solicitud.Destinatario.Email, FotoPerfilUrl = solicitud.Destinatario.FotoPerfilUrl },
                Estado = solicitud.Estado.ToString(),
                FechaEnvio = solicitud.FechaEnvio,
                FechaRespuesta = solicitud.FechaRespuesta,
                Mensaje = solicitud.Mensaje
            };
        }
    }
}
