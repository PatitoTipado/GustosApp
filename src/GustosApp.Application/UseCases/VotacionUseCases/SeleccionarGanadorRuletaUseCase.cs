using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.VotacionUseCases
{
    public class SeleccionarGanadorRuletaUseCase
    {
        private readonly IVotacionRepository _votacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly INotificacionesVotacionService _notificaciones;

        public SeleccionarGanadorRuletaUseCase(
            IVotacionRepository votacionRepository,
            IUsuarioRepository usuarioRepository,
           INotificacionesVotacionService notificaciones)
        {
            _votacionRepository = votacionRepository;
            _usuarioRepository = usuarioRepository;
            _notificaciones = notificaciones;
        }

        public async Task<VotacionGrupo> HandleAsync(
       string firebaseUid,
       Guid votacionId,
       Guid restauranteGanadorId,
       CancellationToken ct = default)
        {
            // 1. Verificar usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // 2. Cargar votación con candidatos
            var votacion = await _votacionRepository.ObtenerPorIdConCandidatosAsync(votacionId, ct)
                ?? throw new ArgumentException("Votación no encontrada");


            // 3. Validar que sea ADMIN DEL GRUPO
            var grupo = votacion.Grupo;
            if (grupo.AdministradorId != usuario.Id)
                throw new UnauthorizedAccessException("Solo el administrador puede seleccionar el ganador");


            // 4. Validar estado de la votación
            if (votacion.Estado != EstadoVotacion.Activa)
                throw new InvalidOperationException("La votación no está activa");


            // 5. Validar que la ruleta solo se pueda usar si NO hay ya un ganador
            if (votacion.RestauranteGanadorId.HasValue)
                throw new InvalidOperationException("Ya se seleccionó un ganador para esta votación");


            // 6. Validar empate real
            var restaurantesEmpatados = votacion.ObtenerRestaurantesEmpatados();
            if (restaurantesEmpatados.Count < 2)
                throw new InvalidOperationException("No hay empate en esta votación");


            // 7. Validar que el restaurante PEDIDO esté entre empatados
            if (!restaurantesEmpatados.Contains(restauranteGanadorId))
                throw new ArgumentException("El restaurante seleccionado no está entre los empatados");


            // 8. Validar que el restaurante sea candidato oficial
            var esCandidato = votacion.RestaurantesCandidatos
                .Any(c => c.RestauranteId == restauranteGanadorId);

            if (!esCandidato)
                throw new InvalidOperationException("El restaurante seleccionado no es un candidato válido");


            // 9. Asignar ganador por ruleta (dominio)
            votacion.EstablecerGanadorRuleta(restauranteGanadorId);

            await _notificaciones.NotificarGanador(votacion.GrupoId, votacion.Id, restauranteGanadorId);


            // 10. Guardar
            await _votacionRepository.ActualizarVotacionAsync(votacion, ct);

            return votacion;
        }
    }
    }
