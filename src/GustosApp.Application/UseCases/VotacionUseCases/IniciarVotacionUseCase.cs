using System;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.VotacionUseCases
{
    public class IniciarVotacionUseCase
    {
        private readonly IVotacionRepository _votacionRepository;
        private readonly IGrupoRepository _grupoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public IniciarVotacionUseCase(
            IVotacionRepository votacionRepository,
            IGrupoRepository grupoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _votacionRepository = votacionRepository;
            _grupoRepository = grupoRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<VotacionGrupo> HandleAsync(
            string firebaseUid,
            Guid grupoId,
            string? descripcion = null,
            CancellationToken ct = default)
        {
            // Verificar que el usuario sea miembro del grupo
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            var esMiembro = await _grupoRepository.UsuarioEsMiembroAsync(grupoId, firebaseUid, ct);
            if (!esMiembro)
                throw new UnauthorizedAccessException("No eres miembro de este grupo");

            // Verificar que no haya una votación activa
            var votacionActiva = await _votacionRepository.ObtenerVotacionActivaAsync(grupoId, ct);
            if (votacionActiva != null)
                throw new InvalidOperationException("Ya existe una votación activa en este grupo");

            // Crear nueva votación
            var votacion = new VotacionGrupo(grupoId, descripcion);
            return await _votacionRepository.CrearVotacionAsync(votacion, ct);
        }
    }
}
