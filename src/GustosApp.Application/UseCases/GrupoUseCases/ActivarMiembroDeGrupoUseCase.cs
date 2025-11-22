using GustosApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.UseCases.GrupoUseCases
{
    public class ActivarMiembroDeGrupoUseCase
    {

        private IGrupoRepository _grupoRepository;
        private IUsuarioRepository _usuarioRepository;
        private IMiembroGrupoRepository _miembroGrupoRepository;
        public ActivarMiembroDeGrupoUseCase(
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
            if (usuarioSolicitante == null || usuarioObtenido == null)
            {
                throw new UnauthorizedAccessException("no existe el usuario");
            }

            if (await _grupoRepository.GetByIdAsync(grupoId)==null)
            {
                throw new KeyNotFoundException("EL grupo no existe");
            }

            var esAdmin = await _grupoRepository.UsuarioEsAdministradorAsync(grupoId, usuarioSolicitante.Id);
            var esElMismoUsuario = usuarioSolicitante.Id.Equals(usuarioId); 

            if (!esAdmin && !esElMismoUsuario)
            {
                throw new UnauthorizedAccessException("Debe ser administrador del grupo o el mismo usuario para activar al miembro.");
            }

            var miembroGrupo = await _miembroGrupoRepository.GetByGrupoYUsuarioAsync(grupoId, usuarioObtenido.IdUsuario);

            if (miembroGrupo == null || miembroGrupo.afectarRecomendacion)
            {
                // Lanza una excepción o devuelve true si ya está activo (Idempotencia)
                if (miembroGrupo != null && miembroGrupo.afectarRecomendacion) return true;

                throw new InvalidOperationException("El usuario no es un miembro inactivo del grupo.");
            }

            //cambiar el estado del usuario en el grupo
            return await _miembroGrupoRepository.ActivarMiembro(grupoId, usuarioObtenido.Id);
        }
    }
}
