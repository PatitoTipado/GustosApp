using GustosApp.Application.DTO;
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

        public AceptarInvitacionGrupoUseCase(IInvitacionGrupoRepository invitacionRepository,
            IUsuarioRepository usuarioRepository,
            IMiembroGrupoRepository miembroGrupoRepository,
            IGustosGrupoRepository gustosGrupoRepository)
        {
            _invitacionRepository = invitacionRepository;
            _usuarioRepository = usuarioRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
            _gustosGrupoRepository = gustosGrupoRepository;
        }

        public async Task<GrupoResponse> HandleAsync(string firebaseUid, Guid invitacionId, CancellationToken cancellationToken = default)
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
            var miembroExistente = await _miembroGrupoRepository.GetByGrupoYUsuarioAsync(invitacion.GrupoId, usuario.Id, cancellationToken);
            
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

            // Obtener el grupo completo con relaciones
            var grupo = await _invitacionRepository.GetByIdAsync(invitacionId, cancellationToken);
            if (grupo?.Grupo == null)
                throw new InvalidOperationException("Error al aceptar la invitación");

            return new GrupoResponse(
                grupo.Grupo.Id,
                grupo.Grupo.Nombre,
                grupo.Grupo.Descripcion,
                grupo.Grupo.AdministradorId,
                grupo.Grupo.Administrador?.FirebaseUid, // Add Firebase UID
                grupo.Grupo.Administrador != null ? (grupo.Grupo.Administrador.Nombre + " " + grupo.Grupo.Administrador.Apellido) : string.Empty,
                grupo.Grupo.FechaCreacion,
                grupo.Grupo.Activo,
                grupo.Grupo.CodigoInvitacion,
                grupo.Grupo.FechaExpiracionCodigo,
                grupo.Grupo.Miembros.Count(m => m.Activo)
            );
        }
    }
}
