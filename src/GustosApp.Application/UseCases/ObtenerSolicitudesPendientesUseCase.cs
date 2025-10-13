using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;

namespace GustosApp.Application.UseCases
{
    public class ObtenerSolicitudesPendientesUseCase
    {
        private readonly ISolicitudAmistadRepository _solicitudRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerSolicitudesPendientesUseCase(ISolicitudAmistadRepository solicitudRepository, IUsuarioRepository usuarioRepository)
        {
            _solicitudRepository = solicitudRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<SolicitudAmistadResponse>> HandleAsync(string firebaseUid, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null) throw new UnauthorizedAccessException("Usuario no encontrado");

            var pendientes = await _solicitudRepository.GetSolicitudesPendientesByUsuarioIdAsync(usuario.Id, cancellationToken);

            return pendientes.Select(p => new SolicitudAmistadResponse
            {
                Id = p.Id,
                Remitente = new UsuarioSimpleResponse { Id = p.Remitente.Id, Nombre = p.Remitente.Nombre + " " + p.Remitente.Apellido, Email = p.Remitente.Email, FotoPerfilUrl = p.Remitente.FotoPerfilUrl },
                Destinatario = new UsuarioSimpleResponse { Id = p.Destinatario.Id, Nombre = p.Destinatario.Nombre + " " + p.Destinatario.Apellido, Email = p.Destinatario.Email, FotoPerfilUrl = p.Destinatario.FotoPerfilUrl },
                Estado = p.Estado.ToString(),
                FechaEnvio = p.FechaEnvio,
                Mensaje = p.Mensaje
            }).ToList();
        }
    }
}
