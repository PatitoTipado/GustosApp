using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.GrupoUseCases
{
    public class ActualizarNombreGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ActualizarNombreGrupoUseCase(
            IGrupoRepository grupoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Grupo> HandleAsync(
            string firebaseUid,
            Guid grupoId,
            string nuevoNombre,
            CancellationToken cancellationToken = default)
        {
            // Validar nombre
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre del grupo no puede estar vacío");

            // Obtener el usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Obtener el grupo
            var grupo = await _grupoRepository.GetByIdAsync(grupoId, cancellationToken);
            if (grupo == null)
                throw new KeyNotFoundException("Grupo no encontrado");

            // Verificar que el usuario es administrador
            if (grupo.AdministradorId != usuario.Id)
                throw new UnauthorizedAccessException("Solo el administrador puede cambiar el nombre del grupo");

            // Actualizar nombre
            grupo.ActualizarNombre(nuevoNombre.Trim());

            // Guardar cambios
            await _grupoRepository.UpdateAsync(grupo, cancellationToken);

            return grupo;
        }
    }
}
