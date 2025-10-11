using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class ObtenerGruposUsuarioUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerGruposUsuarioUseCase(IGrupoRepository grupoRepository, IUsuarioRepository usuarioRepository)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<GrupoResponse>> HandleAsync(string firebaseUid, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Obtener los grupos del usuario
            var grupos = await _grupoRepository.GetGruposByUsuarioIdAsync(usuario.Id, cancellationToken);

            return grupos.Select(grupo => new GrupoResponse(
                grupo.Id,
                grupo.Nombre,
                grupo.Descripcion,
                grupo.AdministradorId,
                grupo.Administrador.Nombre + " " + grupo.Administrador.Apellido,
                grupo.FechaCreacion,
                grupo.Activo,
                grupo.CodigoInvitacion,
                grupo.FechaExpiracionCodigo,
                grupo.Miembros.Count(m => m.Activo)
            ));
        }
    }
}
