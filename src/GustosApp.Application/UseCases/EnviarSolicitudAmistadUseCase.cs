using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class EnviarSolicitudAmistadUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISolicitudAmistadRepository _solicitudRepository;

        public EnviarSolicitudAmistadUseCase(IUsuarioRepository usuarioRepository, ISolicitudAmistadRepository solicitudRepository)
        {
            _usuarioRepository = usuarioRepository;
            _solicitudRepository = solicitudRepository;
        }

        public async Task<SolicitudAmistadResponse> HandleAsync(string firebaseUid, EnviarSolicitudRequest request, CancellationToken cancellationToken = default)
        {
            var remitente = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (remitente == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            var destinatario = await _usuarioRepository.GetByUsernameAsync(request.UsernameDestino, cancellationToken);
            if (destinatario == null)
                throw new ArgumentException("No se encontró un usuario con ese username");

            if (destinatario.Id == remitente.Id)
                throw new ArgumentException("No puedes enviarte una solicitud a ti mismo");

            // Evitar duplicados en ambos sentidos
            var existePendiente = await _solicitudRepository.ExisteSolicitudPendienteAsync(remitente.Id, destinatario.Id, cancellationToken)
                                || await _solicitudRepository.ExisteSolicitudPendienteAsync(destinatario.Id, remitente.Id, cancellationToken);

            if (existePendiente)
                throw new ArgumentException("Ya existe una solicitud pendiente entre estos usuarios");

            // Crear solicitud
            var solicitud = new SolicitudAmistad(remitente.Id, destinatario.Id, request.Mensaje);
            await _solicitudRepository.CreateAsync(solicitud, cancellationToken);

            var completa = await _solicitudRepository.GetByIdAsync(solicitud.Id, cancellationToken);
            if (completa == null)
                throw new InvalidOperationException("Error al crear la solicitud");

            return new SolicitudAmistadResponse
            {
                Id = completa.Id,
                Remitente = new UsuarioSimpleResponse
                {
                    Id = completa.Remitente.Id,
                    Nombre = $"{completa.Remitente.Nombre} {completa.Remitente.Apellido}",
                    Email = completa.Remitente.Email,
                    FotoPerfilUrl = completa.Remitente.FotoPerfilUrl,
                    Username = completa.Remitente.IdUsuario
                },
                Destinatario = new UsuarioSimpleResponse
                {
                    Id = completa.Destinatario.Id,
                    Nombre = $"{completa.Destinatario.Nombre} {completa.Destinatario.Apellido}",
                    Email = completa.Destinatario.Email,
                    FotoPerfilUrl = completa.Destinatario.FotoPerfilUrl,
                    Username = completa.Destinatario.IdUsuario
                },
                Estado = completa.Estado.ToString(),
                FechaEnvio = completa.FechaEnvio,
                Mensaje = completa.Mensaje
            };
        }

    }
}
