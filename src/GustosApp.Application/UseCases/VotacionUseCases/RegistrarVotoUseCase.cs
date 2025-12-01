using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.VotacionUseCases
{
    public class RegistrarVotoUseCase
    {
        private readonly IVotacionRepository _votacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IGrupoRepository _grupoRepository;
        private readonly IRestauranteRepository _restauranteRepository;
        private readonly INotificacionesVotacionService _notificaciones;

        public RegistrarVotoUseCase(
            IVotacionRepository votacionRepository,
            IUsuarioRepository usuarioRepository,
            IGrupoRepository grupoRepository,
            IRestauranteRepository restauranteRepository,
           INotificacionesVotacionService notificaciones)
        {
            _votacionRepository = votacionRepository;
            _usuarioRepository = usuarioRepository;
            _grupoRepository = grupoRepository;
            _restauranteRepository = restauranteRepository;
            _notificaciones = notificaciones;
        }

        public async Task<VotoRestaurante> HandleAsync(
         string firebaseUid,
         Guid votacionId,
         Guid restauranteId,
         string? comentario = null,
         CancellationToken ct = default)
        {
            // 1. Obtener usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // 2. Obtener votación con CANDIDATOS
            var votacion = await _votacionRepository.ObtenerPorIdConCandidatosAsync(votacionId, ct)
                ?? throw new ArgumentException("Votación no encontrada");

            // 3. Verificar estado activo
            if (votacion.Estado != EstadoVotacion.Activa)
                throw new InvalidOperationException("La votación no está activa");

            // 4. Verificar que el usuario es miembro y tiene afectarRecomendacion = true
            var grupo = votacion.Grupo;
            var miembro = grupo.Miembros.FirstOrDefault(m => m.UsuarioId == usuario.Id);

            if (miembro == null)
                throw new UnauthorizedAccessException("No eres miembro de este grupo");

            if (!miembro.afectarRecomendacion)
                throw new InvalidOperationException("No puedes votar porque no estás marcado para asistir a la reunión");

            // 5. VALIDAR QUE EL RESTAURANTE SEA CANDIDATO
            bool esCandidato = votacion.RestaurantesCandidatos
                .Any(rc => rc.RestauranteId == restauranteId);

            if (!esCandidato)
                throw new InvalidOperationException("Este restaurante no es candidato en esta votación.");

            // 6. Verificar que el restaurante existe
            var restaurante = await _restauranteRepository.GetRestauranteByIdAsync(restauranteId, ct);
            if (restaurante == null)
                throw new ArgumentException("Restaurante no encontrado");

            // 7. Verificar si el usuario ya votó
            var votoExistente = await _votacionRepository.ObtenerVotoUsuarioAsync(votacionId, usuario.Id, ct);

            if (votoExistente != null)
            {
                // Actualizar
                votoExistente.ActualizarVoto(restauranteId, comentario);
                await _votacionRepository.ActualizarVotoAsync(votoExistente, ct);

                await _notificaciones.NotificarVotoRegistrado(votacion.GrupoId, votacionId);
                await _notificaciones.NotificarResultadosActualizados(votacion.GrupoId, votacionId);

                return votoExistente;
            }

            // 8. Crear voto nuevo
            var nuevoVoto = new VotoRestaurante(votacionId, usuario.Id, restauranteId, comentario);

            await _notificaciones.NotificarVotoRegistrado(votacion.GrupoId, votacionId);
            await _notificaciones.NotificarResultadosActualizados(votacion.GrupoId, votacionId);

            return await _votacionRepository.RegistrarVotoAsync(nuevoVoto, ct);
        }

    }
}