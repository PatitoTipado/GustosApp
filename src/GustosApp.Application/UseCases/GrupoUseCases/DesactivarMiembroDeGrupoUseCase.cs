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
            // 1. Obtener y Validar Existencia de Usuarios
            var usuarioSolicitante = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid);
            var usuarioADesactivar = await _usuarioRepository.GetByIdAsync(usuarioIdADesactivar);

            // Usamos mensajes más específicos
            if (usuarioSolicitante == null)
            {
                throw new UnauthorizedAccessException("El usuario solicitante no existe.");
            }
            if (usuarioADesactivar == null)
            {
                // El usuario a manipular no existe, esto es una entrada inválida.
                throw new ArgumentException("El ID de usuario a desactivar no existe.", nameof(usuarioIdADesactivar));
            }

            // 2. Validar Existencia del Grupo
            if (await _grupoRepository.GetByIdAsync(grupoId) == null)
            {
                // Usando tu elección de excepción. Se recomienda ArgumentException para IDs inválidas.
                throw new KeyNotFoundException("EL grupo no existe");
            }

            // 3. Lógica de Autorización (Admin O Mismo Usuario)
            var esAdmin = await _grupoRepository.UsuarioEsAdministradorAsync(grupoId, usuarioSolicitante.Id);
            var esElMismoUsuario = usuarioSolicitante.Id.Equals(usuarioADesactivar.Id);

            // Regla de Denegación: NO es Admin Y NO es el mismo usuario.
            if (!esAdmin && !esElMismoUsuario)
            {
                throw new UnauthorizedAccessException("Debe ser administrador del grupo o el mismo usuario para desactivar a un miembro.");
            }

            // *** Validación de Negocio Adicional (Importante para Desactivar) ***
            // Un administrador NO puede desactivarse a sí mismo si es el único administrador del grupo. 
            // Si esta es una regla de negocio, debería ir aquí.

            // 4. Comprobación de Pertenencia y Estado Actual (Idempotencia)
            // Asumiendo que 'afectarRecomendacion' significa 'Activo'.
            var miembroGrupo = await _miembroGrupoRepository.GetByGrupoYUsuarioAsync(grupoId, usuarioADesactivar.IdUsuario);

            if (miembroGrupo == null)
            {
                throw new InvalidOperationException("El usuario a desactivar no es un miembro del grupo.");
            }

            // Si ya está inactivo (no afecta la recomendación), devolver true (Idempotencia)
            if (!miembroGrupo.afectarRecomendacion)
            {
                return true;
            }

            // 5. Ejecutar la acción
            return await _miembroGrupoRepository.DesactivarMiembroDeGrupo(grupoId, usuarioADesactivar.Id);
        }
    }
}
