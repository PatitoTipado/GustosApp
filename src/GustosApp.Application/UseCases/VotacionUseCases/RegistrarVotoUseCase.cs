using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public RegistrarVotoUseCase(
            IVotacionRepository votacionRepository,
            IUsuarioRepository usuarioRepository,
            IGrupoRepository grupoRepository,
            IRestauranteRepository restauranteRepository)
        {
            _votacionRepository = votacionRepository;
            _usuarioRepository = usuarioRepository;
            _grupoRepository = grupoRepository;
            _restauranteRepository = restauranteRepository;
        }

        public async Task<VotoRestaurante> HandleAsync(
            string firebaseUid,
            Guid votacionId,
            Guid restauranteId,
            string? comentario = null,
            CancellationToken ct = default)
        {
            // Obtener usuario
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // Obtener votación
            var votacion = await _votacionRepository.ObtenerPorIdAsync(votacionId, ct)
                ?? throw new ArgumentException("Votación no encontrada");

            // Verificar que la votación esté activa
            if (votacion.Estado != EstadoVotacion.Activa)
                throw new InvalidOperationException("La votación no está activa");

            // Verificar que el usuario sea miembro del grupo y tenga afectarRecomendacion = true
            var grupo = votacion.Grupo;
            var miembro = grupo.Miembros.FirstOrDefault(m => m.UsuarioId == usuario.Id);

            if (miembro == null)
                throw new UnauthorizedAccessException("No eres miembro de este grupo");

            if (!miembro.afectarRecomendacion)
                throw new InvalidOperationException("No puedes votar porque no estás marcado para asistir a la reunión");

            // Verificar que el restaurante existe
            var restaurante = await _restauranteRepository.GetRestauranteByIdAsync(restauranteId, ct);
            if (restaurante == null)
                throw new ArgumentException("Restaurante no encontrado");

            // Verificar si el usuario ya votó
            var votoExistente = await _votacionRepository.ObtenerVotoUsuarioAsync(votacionId, usuario.Id, ct);

            if (votoExistente != null)
            {
                // Actualizar voto existente
                votoExistente.ActualizarVoto(restauranteId, comentario);
                await _votacionRepository.ActualizarVotacionAsync(votacion, ct);
                return votoExistente;
            }
            else
            {
                // Crear nuevo voto
                var nuevoVoto = new VotoRestaurante(votacionId, usuario.Id, restauranteId, comentario);
                return await _votacionRepository.RegistrarVotoAsync(nuevoVoto, ct);
            }
        }
    }
}
