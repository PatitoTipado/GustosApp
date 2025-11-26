using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.VotacionUseCases
{
    public class CerrarVotacionUseCase
    {
        private readonly IVotacionRepository _votacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IGrupoRepository _grupoRepository;

        public CerrarVotacionUseCase(
            IVotacionRepository votacionRepository,
            IUsuarioRepository usuarioRepository,
            IGrupoRepository grupoRepository)
        {
            _votacionRepository = votacionRepository;
            _usuarioRepository = usuarioRepository;
            _grupoRepository = grupoRepository;
        }

        public async Task<VotacionGrupo> HandleAsync(
            string firebaseUid,
            Guid votacionId,
            Guid? restauranteGanadorId = null,
            CancellationToken ct = default)
        {
            // Verificar que el usuario existe
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // Obtener votación
            var votacion = await _votacionRepository.ObtenerPorIdAsync(votacionId, ct)
                ?? throw new ArgumentException("Votación no encontrada");

            // Verificar que el usuario sea administrador del grupo
            var grupo = votacion.Grupo;
            if (grupo.AdministradorId != usuario.Id)
                throw new UnauthorizedAccessException("Solo el administrador puede cerrar la votación");

            // Si no se especifica ganador, calcularlo automáticamente
            if (!restauranteGanadorId.HasValue)
            {
                var resultados = votacion.ObtenerResultados();
                if (resultados.Any())
                {
                    var maxVotos = resultados.Max(r => r.Value);
                    var ganadores = resultados.Where(r => r.Value == maxVotos).Select(r => r.Key).ToList();

                    // Si hay empate, no se asigna ganador automáticamente
                    if (ganadores.Count == 1)
                    {
                        restauranteGanadorId = ganadores.First();
                    }
                }
            }

            // Cerrar votación
            votacion.CerrarVotacion(restauranteGanadorId);
            await _votacionRepository.ActualizarVotacionAsync(votacion, ct);

            return votacion;
        }
    }
}
