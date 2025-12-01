using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Common;
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

            // 4. Verificar que el usuario es miembro
            var grupo = votacion.Grupo;
            var miembro = grupo.Miembros.FirstOrDefault(m => m.UsuarioId == usuario.Id);

            if (miembro == null)
                throw new UnauthorizedAccessException("No eres miembro de este grupo");

            if (!miembro.afectarRecomendacion)
                throw new InvalidOperationException("No puedes votar porque no estás marcado para asistir a la reunión");

            // 5. VALIDAR CANDIDATO
            bool esCandidato = votacion.RestaurantesCandidatos.Any(rc => rc.RestauranteId == restauranteId);
            if (!esCandidato)
                throw new InvalidOperationException("Este restaurante no es candidato en esta votación.");

            // 6. Obtener restaurante
            var restaurante = await _restauranteRepository.GetRestauranteByIdAsync(restauranteId, ct)
                ?? throw new ArgumentException("Restaurante no encontrado");

            // 7. ¿Ya votó?
            var votoExistente = await _votacionRepository.ObtenerVotoUsuarioAsync(votacionId, usuario.Id, ct);

            if (votoExistente != null)
            {
                // --- ACTUALIZAR VOTO ---
                votoExistente.ActualizarVoto(restauranteId, comentario);
                await _votacionRepository.ActualizarVotoAsync(votoExistente, ct);

                var payloadUpdate = new EventoVotoRegistrado
                {
                    VotacionId = votacionId,
                    UsuarioId = usuario.Id,
                    UsuarioNombre = usuario.Nombre,
                    UsuarioFirebaseUid = usuario?.FirebaseUid,
                    UsuarioFoto = usuario.FotoPerfilUrl ?? "",
                    RestauranteId = restaurante.Id,
                    RestauranteNombre = restaurante.Nombre,
                    RestauranteImagen = restaurante.ImagenUrl ?? "",
                    EsActualizacion = true
                };

                await _notificaciones.NotificarVotoRegistrado(votacion.GrupoId, payloadUpdate);
                await _notificaciones.NotificarResultadosActualizados(votacion.GrupoId, votacionId);

                return votoExistente;
            }

            // --- CREAR NUEVO VOTO ---
            var nuevoVoto = new VotoRestaurante(votacionId, usuario.Id, restauranteId, comentario);
            await _votacionRepository.RegistrarVotoAsync(nuevoVoto, ct);

            var payloadNuevo = new EventoVotoRegistrado
            {
                VotacionId = votacionId,
                UsuarioId = usuario.Id,
                UsuarioNombre = usuario.Nombre,
                UsuarioFoto = usuario.FotoPerfilUrl ?? "",
                RestauranteId = restaurante.Id,
                RestauranteNombre = restaurante.Nombre,
                RestauranteImagen = restaurante.ImagenUrl ?? "",
                EsActualizacion = false
            };

            await _notificaciones.NotificarVotoRegistrado(votacion.GrupoId, payloadNuevo);
            await _notificaciones.NotificarResultadosActualizados(votacion.GrupoId, votacionId);

            return nuevoVoto;
        }

    }
}