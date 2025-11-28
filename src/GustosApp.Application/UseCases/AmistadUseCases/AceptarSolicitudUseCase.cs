using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.AmistadUseCases
{
    public class AceptarSolicitudUseCase
    {
        private readonly ISolicitudAmistadRepository _solicitudRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;

        public AceptarSolicitudUseCase(ISolicitudAmistadRepository solicitudRepository, IUsuarioRepository usuarioRepository, IMiembroGrupoRepository miembroGrupoRepository)
        {
            _solicitudRepository = solicitudRepository;
            _usuarioRepository = usuarioRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
        }

        public async Task<SolicitudAmistad> HandleAsync(
        string firebaseUid,
        Guid solicitudId,
        CancellationToken ct = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            var solicitud = await _solicitudRepository.GetByIdAsync(solicitudId, ct)
                ?? throw new ArgumentException("Solicitud no encontrada");

            if (solicitud.DestinatarioId != usuario.Id)
                throw new UnauthorizedAccessException("Solo el destinatario puede aceptar la solicitud");

            solicitud.Aceptar();

            await _solicitudRepository.UpdateAsync(solicitud, ct);


            return solicitud;
        }

    }
}