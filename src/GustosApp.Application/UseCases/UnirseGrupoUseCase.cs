using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class UnirseGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;

        public UnirseGrupoUseCase(IGrupoRepository grupoRepository,
            IUsuarioRepository usuarioRepository,
            IMiembroGrupoRepository miembroGrupoRepository)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
        }

        public async Task<GrupoResponse> HandleAsync(string firebaseUid, UnirseGrupoRequest request, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Buscar el grupo por código de invitación
            var grupo = await _grupoRepository.GetByCodigoInvitacionAsync(request.CodigoInvitacion, cancellationToken);
            if (grupo == null)
                throw new ArgumentException("Código de invitación inválido");

            // Verificar que el código no ha expirado
            if (!grupo.EsCodigoInvitacionValido(request.CodigoInvitacion))
                throw new ArgumentException("El código de invitación ha expirado");

            // Verificar que el usuario no es ya miembro del grupo
            if (await _miembroGrupoRepository.UsuarioEsMiembroActivoAsync(grupo.Id, usuario.Id, cancellationToken))
                throw new ArgumentException("Ya eres miembro de este grupo");

            // Agregar al usuario como miembro del grupo
            var miembro = new MiembroGrupo(grupo.Id, usuario.Id, false);
            await _miembroGrupoRepository.CreateAsync(miembro, cancellationToken);

            // Obtener el grupo completo con relaciones
            var grupoCompleto = await _grupoRepository.GetByIdAsync(grupo.Id, cancellationToken);
            if (grupoCompleto == null)
                throw new InvalidOperationException("Error al unirse al grupo");

            return new GrupoResponse(
                grupoCompleto.Id,
                grupoCompleto.Nombre,
                grupoCompleto.Descripcion,
                grupoCompleto.AdministradorId,
                grupoCompleto.Administrador.Nombre + " " + grupoCompleto.Administrador.Apellido,
                grupoCompleto.FechaCreacion,
                grupoCompleto.Activo,
                grupoCompleto.CodigoInvitacion,
                grupoCompleto.FechaExpiracionCodigo,
                grupoCompleto.Miembros.Count(m => m.Activo)
            );
        }
    }
}
