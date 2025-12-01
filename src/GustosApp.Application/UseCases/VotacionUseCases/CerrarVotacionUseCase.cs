using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.VotacionUseCases
{
    public class CerrarVotacionUseCase
    {
        private readonly IVotacionRepository _votacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IGrupoRepository _grupoRepository;
        private readonly INotificacionesVotacionService _notificaciones;

        public CerrarVotacionUseCase(
            IVotacionRepository votacionRepository,
            IUsuarioRepository usuarioRepository,
            IGrupoRepository grupoRepository,
            INotificacionesVotacionService notificaciones)
        {
            _votacionRepository = votacionRepository;
            _usuarioRepository = usuarioRepository;
            _grupoRepository = grupoRepository;
            _notificaciones = notificaciones;
        }

        public async Task<VotacionGrupo> HandleAsync(
      string firebaseUid,
      Guid votacionId,
      Guid? restauranteGanadorId = null,
      CancellationToken ct = default)
        {
            // 1. Validar usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // 2. Obtener votación con candidatos
            var votacion = await _votacionRepository.ObtenerPorIdConCandidatosAsync(votacionId, ct)
                ?? throw new ArgumentException("Votación no encontrada");

            var grupo = votacion.Grupo;

            // 3. Validar que sea administrador
            if (grupo.AdministradorId != usuario.Id)
                throw new UnauthorizedAccessException("Solo el administrador puede cerrar la votación");

            if (votacion.Estado != EstadoVotacion.Activa)
                throw new InvalidOperationException("La votación no está activa");


            // 4. Si se manda ganador → VALIDAR QUE SEA CANDIDATO
            if (restauranteGanadorId.HasValue)
            {
                var esCandidato = votacion.RestaurantesCandidatos
                    .Any(rc => rc.RestauranteId == restauranteGanadorId);

                if (!esCandidato)
                    throw new InvalidOperationException("El ganador debe ser un restaurante candidato");
            }

            // 5. Si NO se envió ganador → calcular automáticamente
            if (!restauranteGanadorId.HasValue)
            {
                var resultados = votacion.ObtenerResultados();

                if (resultados.Any())
                {
                    var maxVotos = resultados.Max(r => r.Value);
                    var ganadores = resultados
                        .Where(r => r.Value == maxVotos)
                        .Select(r => r.Key)
                        .ToList();

                    // Si hay un único ganador → asignarlo
                    if (ganadores.Count == 1)
                    {
                        restauranteGanadorId = ganadores.First();
                    }
                    else
                    {
                        // Si hay más de uno → empate
                        restauranteGanadorId = null;
                    }
                }
            }

            // 6. Cerrar votación
            votacion.CerrarVotacion(restauranteGanadorId);

            // 7. Guardar
            await _votacionRepository.ActualizarVotacionAsync(votacion, ct);

            await _notificaciones.NotificarVotacionCerrada(votacion.GrupoId, votacion.Id, restauranteGanadorId);


            return votacion;
        }

    }
    }
