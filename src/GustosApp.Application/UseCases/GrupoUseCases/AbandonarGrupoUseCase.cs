using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.GrupoUseCases
{
    public class AbandonarGrupoUseCase
    {
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMiembroGrupoRepository _miembroGrupoRepository;
        private readonly IChatRealTimeService _chatRealTime;

        public AbandonarGrupoUseCase(IGrupoRepository grupoRepository,
            IUsuarioRepository usuarioRepository,
            IMiembroGrupoRepository miembroGrupoRepository,
             IChatRealTimeService chatRealTime)
        {
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
            _chatRealTime = chatRealTime;
        }

        public async Task<bool> HandleAsync(string firebaseUid, Guid grupoId, CancellationToken cancellationToken = default)
        {
            // Obtener el usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, cancellationToken);
            if (usuario == null)
                throw new UnauthorizedAccessException("Usuario no encontrado");

            // Verificar que el grupo existe
            var grupo = await _grupoRepository.GetByIdAsync(grupoId, cancellationToken);
            if (grupo == null)
                throw new ArgumentException("Grupo no encontrado");

            // Verificar que el usuario es miembro del grupo
            var miembro = await _miembroGrupoRepository.GetByGrupoYUsuarioAsync(grupoId, usuario.IdUsuario, cancellationToken);
            if (miembro == null || !miembro.Activo)
                throw new ArgumentException("No eres miembro de este grupo");

            // Verificar que no es el único administrador
            if (miembro.EsAdministrador)
            {
                var cantidadAdministradores = await _miembroGrupoRepository.GetMiembrosByGrupoIdAsync(grupoId, cancellationToken);
                var administradoresActivos = cantidadAdministradores.Count(m => m.EsAdministrador && m.Activo);

                if (administradoresActivos <= 1)
                    throw new InvalidOperationException("No puedes abandonar el grupo siendo el único administrador. Promueve a otro miembro a administrador primero.");
            }

            // Abandonar el grupo
            miembro.AbandonarGrupo();
            await _miembroGrupoRepository.UpdateAsync(miembro, cancellationToken);
         

            await _chatRealTime.UsuarioAbandono(grupoId, usuario.Id,usuario.IdUsuario,usuario.FirebaseUid);

            return true;
        }
    }
}
