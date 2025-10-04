using GustosApp.Application.DTO;
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

        public InvitarUsuarioGrupoUseCase(IGrupoRepository grupoRepository,
            IUsuarioRepository usuarioRepository,
            IInvitacionGrupoRepository invitacionRepository,
            IMiembroGrupoRepository miembroGrupoRepository)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
            _invitacionRepository = invitacionRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
        }

        public async Task<InvitacionGrupoResponse> HandleAsync(string firebaseUid, Guid grupoId, InvitacionGrupoRequest request, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario invitador
            var usuarioInvitador = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuarioInvitador == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Verificar que el grupo existe
            var grupo = await _grupoRepository.GetByIdAsync(grupoId, cancellationToken);
            if (grupo == null)
                throw new ArgumentException("Grupo no encontrado");

            // Verificar que el usuario es administrador del grupo
            if (!await _grupoRepository.UsuarioEsAdministradorAsync(grupoId, usuarioInvitador.Id, cancellationToken))
                throw new UnauthorizedAccessException("Solo los administradores pueden invitar usuarios");

            // Buscar el usuario a invitar por email
            var usuarioInvitado = await _usuarioRepository.GetByEmailAsync(request.EmailUsuario, cancellationToken);
            if (usuarioInvitado == null)
                throw new ArgumentException("Usuario no encontrado con ese email");

            // Verificar que no es el mismo usuario
            if (usuarioInvitado.Id == usuarioInvitador.Id)
                throw new ArgumentException("No puedes invitarte a ti mismo");

            // Verificar que el usuario no es ya miembro del grupo
            if (await _miembroGrupoRepository.UsuarioEsMiembroActivoAsync(grupoId, usuarioInvitado.Id, cancellationToken))
                throw new ArgumentException("El usuario ya es miembro del grupo");

            // Verificar que no hay una invitación pendiente
            if (await _invitacionRepository.ExisteInvitacionPendienteAsync(grupoId, usuarioInvitado.Id, cancellationToken))
                throw new ArgumentException("Ya existe una invitación pendiente para este usuario");

            // Crear la invitación
            var invitacion = new InvitacionGrupo(grupoId, usuarioInvitado.Id, usuarioInvitador.Id, request.MensajePersonalizado);
            await _invitacionRepository.CreateAsync(invitacion, cancellationToken);

            // Obtener la invitación completa con relaciones
            var invitacionCompleta = await _invitacionRepository.GetByIdAsync(invitacion.Id, cancellationToken);
            if (invitacionCompleta == null)
                throw new InvalidOperationException("Error al crear la invitación");

            return new InvitacionGrupoResponse(
                invitacionCompleta.Id,
                invitacionCompleta.GrupoId,
                invitacionCompleta.Grupo.Nombre,
                invitacionCompleta.UsuarioInvitadoId,
                invitacionCompleta.UsuarioInvitado.Nombre + " " + invitacionCompleta.UsuarioInvitado.Apellido,
                invitacionCompleta.UsuarioInvitado.Email,
                invitacionCompleta.UsuarioInvitadorId,
                invitacionCompleta.UsuarioInvitador.Nombre + " " + invitacionCompleta.UsuarioInvitador.Apellido,
                invitacionCompleta.FechaInvitacion,
                invitacionCompleta.FechaRespuesta,
                invitacionCompleta.Estado.ToString(),
                invitacionCompleta.MensajePersonalizado,
                invitacionCompleta.FechaExpiracion
            );
        }
    }
}
