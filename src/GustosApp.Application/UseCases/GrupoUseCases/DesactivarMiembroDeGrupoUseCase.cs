using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.GrupoUseCases
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

            var usuarioSolicitante = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid);
            var usuarioObtenido = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (usuarioSolicitante== null || usuarioObtenido == null)
            {
                throw new UnauthorizedAccessException("no existe el usuario");
            }

            var esAdmin = await _grupoRepository.UsuarioEsAdministradorAsync(grupoId, usuarioSolicitante.Id);

            var esMiembro = await _grupoRepository.UsuarioEsMiembroAsync(grupoId, firebaseUid);

            if (!esAdmin ||!usuarioObtenido.Id.Equals(usuarioId) && esMiembro)
                throw new UnauthorizedAccessException("No tienes permisos para desactivar al usuario");

            //cambiar el estado del usuario en el grupo
            return await _miembroGrupoRepository.DesactivarMiembroDeGrupo(grupoId, usuarioObtenido.Id);
        }
    }
}
