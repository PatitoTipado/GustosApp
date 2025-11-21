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

        public async Task<bool> Handle(Guid grupoId, Guid usuarioIdADesactivar, string firebaseUid)
        {
            var usuarioSolicitante = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid);
            var usuarioADesactivar = await _usuarioRepository.GetByIdAsync(usuarioIdADesactivar);

            if (usuarioSolicitante == null)
            {
                throw new UnauthorizedAccessException("El usuario solicitante no existe.");
            }
            if (usuarioADesactivar == null)
            {
                throw new ArgumentException("El ID de usuario a desactivar no existe.", nameof(usuarioIdADesactivar));
            }

            if (await _grupoRepository.GetByIdAsync(grupoId) == null)
            {
                throw new KeyNotFoundException("EL grupo no existe");
            }

            var esAdmin = await _grupoRepository.UsuarioEsAdministradorAsync(grupoId, usuarioSolicitante.Id);
            var esElMismoUsuario = usuarioSolicitante.Id.Equals(usuarioADesactivar.Id);

            if (!esAdmin && !esElMismoUsuario)
            {
                throw new UnauthorizedAccessException("Debe ser administrador del grupo o el mismo usuario para desactivar a un miembro.");
            }            

            var miembroGrupo = await _miembroGrupoRepository.GetByGrupoYUsuarioAsync(grupoId, usuarioADesactivar.IdUsuario);

            if (miembroGrupo == null)
            {
                throw new InvalidOperationException("El usuario a desactivar no es un miembro del grupo.");
            }

            if (!miembroGrupo.afectarRecomendacion)
            {
                return true;
            }

            return await _miembroGrupoRepository.DesactivarMiembroDeGrupo(grupoId, usuarioADesactivar.Id);
        }
    }
}
