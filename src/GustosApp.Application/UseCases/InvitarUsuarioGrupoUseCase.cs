using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class InvitarUsuarioGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IInvitacionGrupoRepository _invitacionRepository;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;
        private readonly INotificacionRepository _notificacionRepository;
        private readonly INotificacionRealtimeService _notificacionRealtimeService;
        public InvitarUsuarioGrupoUseCase(IGrupoRepository grupoRepository,
            IUsuarioRepository usuarioRepository,
            IInvitacionGrupoRepository invitacionRepository,
            IMiembroGrupoRepository miembroGrupoRepository,
            INotificacionRepository notificacionRepository,
            INotificacionRealtimeService notificacionRealtimeService)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
            _invitacionRepository = invitacionRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
            _notificacionRepository = notificacionRepository;
            _notificacionRealtimeService = notificacionRealtimeService;

        }

        public async Task<InvitacionGrupo> HandleAsync(
           string firebaseUid,Guid grupoId,string? EmailUsuario, Guid? UsuarioId,
           string? UsuarioUsername, string? MensajePersonalizado,
           CancellationToken ct = default)
        {
            // 1. Usuario invitador
            var usuarioInvitador = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // 2. Grupo
            var grupo = await _grupoRepository.GetByIdAsync(grupoId, ct)
                ?? throw new ArgumentException("Grupo no encontrado");

            // 3. Solo administradores pueden invitar
            if (!await _grupoRepository.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, ct))
                throw new UnauthorizedAccessException("Solo los administradores pueden invitar usuarios");

            // 4. Buscar usuario invitado
            var usuarioInvitado = await ObtenerUsuarioInvitado(EmailUsuario,UsuarioId,
           UsuarioUsername, MensajePersonalizado, ct)
                ?? throw new ArgumentException("No se encontró el usuario a invitar");

            if (usuarioInvitado.Id == usuarioInvitador.Id)
                throw new ArgumentException("No puedes invitarte a ti mismo");

            if (await _miembroGrupoRepository.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, ct))
                throw new ArgumentException("El usuario ya es miembro del grupo");

            if (await _invitacionRepository.ExisteInvitacionPendienteAsync(grupoId, usuarioInvitado.Id, ct))
                throw new ArgumentException("Ya existe una invitación pendiente para este usuario");

            // 5. Crear notificación
            var notificacion = new Notificacion
            {
                UsuarioDestinoId = usuarioInvitado.Id,
                Titulo = "Invitación a grupo",
                Mensaje = $"{usuarioInvitador.Nombre} te ha invitado a unirte al grupo '{grupo.Nombre}'",
                Tipo = TipoNotificacion.InvitacionGrupo,
                Leida = false,
                FechaCreacion = DateTime.UtcNow,
            };
            await _notificacionRepository.crearAsync(notificacion, ct);

            // 6. Crear invitación y vincular notificación
            var invitacion = new InvitacionGrupo(
                grupoId,
                usuarioInvitado.Id,
                usuarioInvitador.Id,
                MensajePersonalizado)
            {
                NotificacionId = notificacion.Id
            };
            await _invitacionRepository.CreateAsync(invitacion, ct);

            // 7. Enlazar y actualizar notificación
            notificacion.InvitacionId = invitacion.Id;
            await _notificacionRepository.UpdateAsync(notificacion, ct);

            // 8. Enviar notificación en tiempo real
            await _notificacionRealtimeService.EnviarNotificacionAsync(
                usuarioInvitado.FirebaseUid,
                notificacion.Titulo,
                notificacion.Mensaje,
                notificacion.Tipo.ToString(),
                ct,
                notificacion.Id,
                invitacion.Id
            );

            // 9. Obtener invitación completa
            var invitacionCompleta = await _invitacionRepository.GetByIdAsync(invitacion.Id, ct)
                ?? throw new InvalidOperationException("Error al crear la invitación");

            return invitacionCompleta;
        }
        private async Task<Usuario?> ObtenerUsuarioInvitado(string? EmailUsuario, Guid? UsuarioId,
           string? UsuarioUsername, string? MensajePersonalizado, CancellationToken ct)
        {
            if (UsuarioId.HasValue && UsuarioId != Guid.Empty)
                return await _usuarioRepository.GetByIdAsync(UsuarioId.Value, ct);

            if (!string.IsNullOrWhiteSpace(UsuarioUsername))
                return await _usuarioRepository.GetByUsernameAsync(UsuarioUsername, ct);

            if (!string.IsNullOrWhiteSpace(EmailUsuario))
                return await _usuarioRepository.GetByEmailAsync(EmailUsuario, ct);

            return null;
        }
    }
}
    
