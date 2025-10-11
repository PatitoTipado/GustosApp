using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class ObtenerInvitacionesUsuarioUseCase
    {
        private readonly IInvitacionGrupoRepository _invitacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerInvitacionesUsuarioUseCase(IInvitacionGrupoRepository invitacionRepository, IUsuarioRepository usuarioRepository)
        {
            _invitacionRepository = invitacionRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<InvitacionGrupoResponse>> HandleAsync(string firebaseUid, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Obtener las invitaciones del usuario
            var invitaciones = await _invitacionRepository.GetInvitacionesPendientesByUsuarioIdAsync(usuario.Id, cancellationToken);

            return invitaciones.Select(invitacion => new InvitacionGrupoResponse(
                invitacion.Id,
                invitacion.GrupoId,
                invitacion.Grupo.Nombre,
                invitacion.UsuarioInvitadoId,
                invitacion.UsuarioInvitado.Nombre + " " + invitacion.UsuarioInvitado.Apellido,
                invitacion.UsuarioInvitado.Email,
                invitacion.UsuarioInvitadorId,
                invitacion.UsuarioInvitador.Nombre + " " + invitacion.UsuarioInvitador.Apellido,
                invitacion.FechaInvitacion,
                invitacion.FechaRespuesta,
                invitacion.Estado.ToString(),
                invitacion.MensajePersonalizado,
                invitacion.FechaExpiracion
            ));
        }
    }
}
