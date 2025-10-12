using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases
{
    public class RemoverMiembroGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;

        public RemoverMiembroGrupoUseCase(IGrupoRepository grupoRepository,
            IUsuarioRepository usuarioRepository,
            IMiembroGrupoRepository miembroGrupoRepository)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
        }

        public async Task<bool> HandleAsync(string firebaseUid, Guid grupoId, Guid usuarioIdARemover, CancellationToken cancellationToken = default)
        {
            // obtener quien realiza la acción
            var usuarioActor = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuarioActor == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            var grupo = await _grupoRepository.GetByIdAsync(grupoId, cancellationToken);
            if (grupo == null)
                throw new ArgumentException("Grupo no encontrado");

            // verificar que actor es administrador
            if (!await _grupoRepository.UsuarioEsAdministradorAsync(grupoId, usuarioActor.Id, cancellationToken))
                throw new UnauthorizedAccessException("Solo los administradores pueden eliminar miembros");

            // buscar el miembro a remover
            var miembro = await _miembroGrupoRepository.GetByGrupoYUsuarioAsync(grupoId, usuarioIdARemover, cancellationToken);
            if (miembro == null || !miembro.Activo)
                throw new ArgumentException("El usuario no es miembro activo del grupo");

            // no permitir eliminar al único administrador
            if (miembro.EsAdministrador)
            {
                var miembros = await _miembroGrupoRepository.GetMiembrosByGrupoIdAsync(grupoId, cancellationToken);
                var administradoresActivos = miembros.Count(m => m.EsAdministrador && m.Activo);
                if (administradoresActivos <= 1)
                    throw new InvalidOperationException("No puedes eliminar al único administrador del grupo");
            }

            // marcar como abandonado (inactivar)
            miembro.AbandonarGrupo();
            await _miembroGrupoRepository.UpdateAsync(miembro, cancellationToken);

            return true;
        }
    }
}
