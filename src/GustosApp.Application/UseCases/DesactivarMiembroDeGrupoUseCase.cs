using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases
{
    public class DesactivarMiembroDeGrupoUseCase
    {
        private IGrupoRepository _grupoRepository;
        private IUsuarioRepository _usuarioRepository;
        private IMiembroGrupoRepository _miembroGrupoRepository;
        public DesactivarMiembroDeGrupoUseCase(
            IGrupoRepository grupo,
            IUsuarioRepository usuarioRepository,
            IMiembroGrupoRepository miembroGrupoRepository)
        {
            _grupoRepository = grupo;
            _usuarioRepository = usuarioRepository;
            _miembroGrupoRepository = miembroGrupoRepository;
        }

        public async Task<bool> Handle(Guid grupoId, Guid usuarioId, string firebaseUid)
        {
            var esAdmin = await _grupoRepository.UsuarioEsAdministradorAsync(grupoId, usuarioId);
            var usuarioObtenido = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid);
            var esMiembro = await _grupoRepository.UsuarioEsMiembroAsync(grupoId, usuarioId);
            if (!esAdmin || (usuarioObtenido!=null && usuarioObtenido.Id.Equals(usuarioId) && esMiembro))
                throw new UnauthorizedAccessException("No tienes permisos para desactivar al usuario no sos el admin");

            var miembro= _miembroGrupoRepository.GetByGrupoYUsuarioAsync(grupoId, usuarioId);

            miembro.Result.AbandonarGrupo();

            //cambiar el estado del usuario en el grupo
            return true;
        }
    }
}
