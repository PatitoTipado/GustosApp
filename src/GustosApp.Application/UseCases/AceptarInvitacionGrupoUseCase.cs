using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class AceptarInvitacionGrupoUseCase
    {
        private readonly IInvitacionGrupoRepository _invitacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;
        private IGustosGrupoRepository _gustosGrupoRepository;
        private readonly EliminarNotificacionUseCase _eliminarNotificacion;
        private readonly INotificacionRealtimeService _notificacionRealtimeService;
        private readonly IGrupoRepository _grupoRepository;

        public AceptarInvitacionGrupoUseCase(IInvitacionGrupoRepository invitacionRepository,
            IUsuarioRepository usuarioRepository,
            IMiembroGrupoRepository miembroGrupoRepository,
            IGustosGrupoRepository gustosGrupoRepository,
            EliminarNotificacionUseCase eliminarNotificacion,
            INotificacionRealtimeService notificacionRealtimeService,
            IGrupoRepository grupoRepository)
        {
            _invitacionRepository = invitacionRepository;
            _usuarioRepository = usuarioRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
            _gustosGrupoRepository = gustosGrupoRepository;
            _eliminarNotificacion = eliminarNotificacion;
            _notificacionRealtimeService = notificacionRealtimeService;
            _grupoRepository = grupoRepository;
        }

        public async Task<Grupo> HandleAsync(string firebaseUid, Guid invitacionId, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Obtener la invitación
            var invitacion = await _invitacionRepository.GetByIdAsync(invitacionId, cancellationToken);
            if (invitacion == null)
                throw new ArgumentException("Invitación no encontrada");

            // Verificar que la invitación es para este usuario
            if (invitacion.UsuarioInvitadoId != usuario.Id)
                throw new UnauthorizedAccessException("Esta invitación no es para ti");

            // Verificar que la invitación está pendiente
            if (invitacion.Estado != EstadoInvitacion.Pendiente)
                throw new ArgumentException("Esta invitación ya fue procesada");

            // Verificar que no ha expirado
            if (invitacion.EstaExpirada())
                throw new ArgumentException("La invitación ha expirado");

            // Verificar que el usuario no es ya miembro activo del grupo
            if (await _miembroGrupoRepository.UsuarioEsMiembroActivoAsync(invitacion.GrupoId, usuario.Id, cancellationToken))
                throw new ArgumentException("Ya eres miembro de este grupo");

            // Aceptar la invitación
            invitacion.Aceptar();
            await _invitacionRepository.UpdateAsync(invitacion, cancellationToken);

            // Verificar si ya existe un registro de miembro (podría estar inactivo)
            var miembroExistente = await _miembroGrupoRepository.GetByGrupoYUsuarioAsync(invitacion.GrupoId, usuario.IdUsuario, cancellationToken);

            if (miembroExistente != null)
            {
                // Reactivar el miembro existente
                miembroExistente.Reincorporar();
                await _miembroGrupoRepository.UpdateAsync(miembroExistente, cancellationToken);
            }
            else
            {
                // Agregar al usuario como miembro del grupo
                var miembro = new MiembroGrupo(invitacion.GrupoId, usuario.Id, false);
                await _miembroGrupoRepository.CreateAsync(miembro, cancellationToken);
                await _gustosGrupoRepository.AgregarGustosAlGrupo(invitacion.GrupoId, usuario.Gustos.ToList(),miembro.Id);
            }


            if (invitacion.NotificacionId != null && invitacion.NotificacionId.HasValue)
            {
                await _eliminarNotificacion.HandleAsync(invitacion.NotificacionId.Value, cancellationToken);

                await _notificacionRealtimeService.EnviarNotificacionAsync(

                    usuario.FirebaseUid,
                    "NotificacionEliminada",
                    invitacion.NotificacionId.Value.ToString(),
                    "InvitacionGrupoEliminada",
                    cancellationToken,
                    invitacion.NotificacionId.Value
                );
            }



            // Recupera el grupo actualizado con relaciones
            var grupo = invitacion.Grupo ?? await _grupoRepository.GetByIdAsync
                (invitacion.GrupoId, cancellationToken)
                ?? throw new InvalidOperationException("Error al aceptar la invitación");

            return grupo;



           
        }
    }
}
