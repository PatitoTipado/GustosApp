using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.VotacionUseCases
{
    public class SeleccionarGanadorRuletaUseCase
    {
        private readonly IVotacionRepository _votacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public SeleccionarGanadorRuletaUseCase(
            IVotacionRepository votacionRepository,
            IUsuarioRepository usuarioRepository)
        {
            _votacionRepository = votacionRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<VotacionGrupo> HandleAsync(
            string firebaseUid,
            Guid votacionId,
            Guid restauranteGanadorId,
            CancellationToken ct = default)
        {
            // Verificar que el usuario existe
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // Obtener votación
            var votacion = await _votacionRepository.ObtenerPorIdAsync(votacionId, ct)
                ?? throw new ArgumentException("Votación no encontrada");

            // Verificar que el usuario sea miembro del grupo
            var grupo = votacion.Grupo;
            if (!grupo.Miembros.Any(m => m.UsuarioId == usuario.Id))
                throw new UnauthorizedAccessException("No eres miembro de este grupo");

            // Verificar que la votación esté activa
            if (votacion.Estado != EstadoVotacion.Activa)
                throw new InvalidOperationException("La votación no está activa");

            // Verificar que hay empate
            var restaurantesEmpatados = votacion.ObtenerRestaurantesEmpatados();
            if (restaurantesEmpatados.Count == 0)
                throw new InvalidOperationException("No hay empate en esta votación");

            // Verificar que no se haya seleccionado un ganador ya
            if (votacion.RestauranteGanadorId.HasValue)
                throw new InvalidOperationException("Ya se seleccionó un ganador para esta votación");

            // Verificar que el restaurante seleccionado está entre los empatados
            if (!restaurantesEmpatados.Contains(restauranteGanadorId))
                throw new ArgumentException("El restaurante seleccionado no está entre los empatados");

            // Asignar ganador de ruleta usando el método del dominio
            votacion.EstablecerGanadorRuleta(restauranteGanadorId);
            await _votacionRepository.ActualizarVotacionAsync(votacion, ct);

            return votacion;
        }
    }
}
