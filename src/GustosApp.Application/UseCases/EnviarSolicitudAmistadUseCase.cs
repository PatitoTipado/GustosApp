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

        public async Task<SolicitudAmistad> HandleAsync(
             string firebaseUid,
             string usernameDestino,
             string? mensaje,
             CancellationToken ct = default)
        {
            var remitente = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            var destinatario = await _usuarioRepository.GetByUsernameAsync(usernameDestino, ct)
                ?? throw new ArgumentException("No se encontró un usuario con ese username");

            if (destinatario.Id == remitente.Id)
                throw new ArgumentException("No puedes enviarte una solicitud a ti mismo");

            var existePendiente =
                await _solicitudRepository.ExisteSolicitudPendienteAsync(remitente.Id, destinatario.Id, ct) ||
                await _solicitudRepository.ExisteSolicitudPendienteAsync(destinatario.Id, remitente.Id, ct);

            if (existePendiente)
                throw new ArgumentException("Ya existe una solicitud pendiente entre estos usuarios");

            var solicitud = new SolicitudAmistad(remitente.Id, destinatario.Id, mensaje);

            await _solicitudRepository.CreateAsync(solicitud, ct);


            var completa = await _solicitudRepository.GetByIdAsync(solicitud.Id, ct)
                ?? throw new InvalidOperationException("Error al crear la solicitud");

            return completa;
        }
    }
    }
