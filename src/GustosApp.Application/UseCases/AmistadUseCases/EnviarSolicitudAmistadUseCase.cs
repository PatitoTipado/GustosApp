using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.Application.UseCases.AmistadUseCases
{
    public class EnviarSolicitudAmistadUseCase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ISolicitudAmistadRepository _solicitudRepository;
        private readonly ISolicitudAmistadRealtimeService _realtimeService;


        public EnviarSolicitudAmistadUseCase(IUsuarioRepository usuarioRepository,
            ISolicitudAmistadRepository solicitudRepository, ISolicitudAmistadRealtimeService realtimeservice
            )
        {
            _usuarioRepository = usuarioRepository;
            _solicitudRepository = solicitudRepository;
            _realtimeService = realtimeservice;

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

            if (remitente.Id == destinatario.Id)
                throw new ArgumentException("No puedes enviarte una solicitud a ti mismo");


            var amistad = await _solicitudRepository
                .GetAmistadEntreUsuariosAsync(remitente.Id, destinatario.Id, ct);

            if (amistad != null && amistad.Estado == EstadoSolicitud.Aceptada)
                throw new ArgumentException("Ya son amigos");


            var hayPendienteA = await _solicitudRepository
                .ExisteSolicitudPendienteAsync(remitente.Id, destinatario.Id, ct);

            var hayPendienteB = await _solicitudRepository
                .ExisteSolicitudPendienteAsync(destinatario.Id, remitente.Id, ct);

            if (hayPendienteA || hayPendienteB)
                throw new ArgumentException("Ya existe una solicitud pendiente entre ustedes");

            var solicitud = new SolicitudAmistad(remitente.Id, destinatario.Id, mensaje);
            await _solicitudRepository.CreateAsync(solicitud, ct);

            var completa = await _solicitudRepository.GetByIdAsync(solicitud.Id, ct)
                ?? throw new InvalidOperationException("Error al crear la solicitud");


            await _realtimeService.EnviarSolicitudAsync(destinatario.FirebaseUid, completa, ct);

            return completa;
        }
    }
    }
