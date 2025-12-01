using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;

namespace GustosApp.Application.UseCases.VotacionUseCases
{
    public class ObtenerResultadosVotacionUseCase
    {
        private readonly IVotacionRepository _votacionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IGrupoRepository _grupoRepository;
        private readonly INotificacionesVotacionService _notificaciones;

        public ObtenerResultadosVotacionUseCase(
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

        public async Task<ResultadoVotacion> HandleAsync(
            string firebaseUid,
            Guid votacionId,
            CancellationToken ct = default)
        {
            // Verificar que el usuario existe
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // Obtener votación
            var votacion = await _votacionRepository.ObtenerPorIdConCandidatosAsync(votacionId, ct)
                ?? throw new ArgumentException("Votación no encontrada");

            var miembro = votacion.Grupo.Miembros.FirstOrDefault(m => m.UsuarioId == usuario.Id);

            if (miembro == null || !miembro.Activo)
                throw new UnauthorizedAccessException("No eres un miembro activo del grupo.");

            // Verificar que el usuario sea miembro del grupo
            var esMiembro = await _grupoRepository.UsuarioEsMiembroAsync(votacion.GrupoId, firebaseUid, ct);
            if (!esMiembro)
                throw new UnauthorizedAccessException("No eres miembro de este grupo");

            // Obtener resultados
            var resultados = votacion.ObtenerResultados();
            var miembrosActivos = votacion.Grupo.Miembros.Count(m => m.afectarRecomendacion && m.Activo);
            var todosVotaron = votacion.TodosHanVotado(miembrosActivos);

            var candidatos = votacion.RestaurantesCandidatos
            .Select(rc => new RestauranteCandidato
             {
                   RestauranteId = rc.RestauranteId,
                  Nombre = rc.Restaurante?.Nombre ?? "",
                  Direccion = rc.Restaurante?.Direccion ?? "",
                      ImagenUrl = rc.Restaurante?.ImagenUrl ?? ""
                      })
             .ToList();


            // Obtener información de los restaurantes votados
            var restaurantesVotados = votacion.Votos
                .Select(v => new RestauranteVotado
                {
                    RestauranteId = v.RestauranteId,
                    RestauranteNombre = v.Restaurante?.Nombre ?? "",
                    RestauranteDireccion = v.Restaurante?.Direccion ?? "",
                    RestauranteImagenUrl = v.Restaurante?.ImagenUrl ?? "",
                    CantidadVotos = resultados.ContainsKey(v.RestauranteId) ? resultados[v.RestauranteId] : 0,
                    Votantes = votacion.Votos
                        .Where(vo => vo.RestauranteId == v.RestauranteId)
                        .Select(vo => new VotanteInfo
                        {
                            UsuarioId = vo.UsuarioId,
                            FirebaseUid = vo.Usuario?.FirebaseUid ,
                            UsuarioNombre = vo.Usuario?.Nombre ?? "",
                            UsuarioFoto = vo.Usuario?.FotoPerfilUrl ?? "",
                            Comentario = vo.Comentario
                        })
                        .ToList()
                })
                .GroupBy(r => r.RestauranteId)
                .Select(g => g.First())
                .OrderByDescending(r => r.CantidadVotos)
                .ToList();

            // Determinar ganador o empate
            // PRIMERO: verificar si ya hay un ganador seleccionado por ruleta
            Guid? ganadorId = votacion.RestauranteGanadorId;
            List<Guid> empatados = new List<Guid>();

            // Si no hay ganador por ruleta, calcularlo por votos
            if (!ganadorId.HasValue && todosVotaron && resultados.Any())
            {
                var maxVotos = resultados.Max(r => r.Value);
                var restaurantesConMaxVotos = resultados.Where(r => r.Value == maxVotos).Select(r => r.Key).ToList();

                if (restaurantesConMaxVotos.Count == 1)
                {
                    ganadorId = restaurantesConMaxVotos.First();
                }
                else
                {
                    empatados = restaurantesConMaxVotos;
                    await _notificaciones.NotificarEmpate(
                    votacion.GrupoId,
                       votacion.Id
                       );
                }
            }

            return new ResultadoVotacion
            {
                VotacionId = votacion.Id,
                GrupoId = votacion.GrupoId,
                Estado = votacion.Estado,
                TodosVotaron = todosVotaron,
                MiembrosActivos = miembrosActivos,
                TotalVotos = votacion.Votos.Count,
                RestaurantesVotados = restaurantesVotados,

                RestaurantesCandidatos = candidatos,

                GanadorId = ganadorId,
                HayEmpate = empatados.Count > 1,
                RestaurantesEmpatados = empatados,
                FechaInicio = votacion.FechaInicio,
                FechaCierre = votacion.FechaCierre
            };

        }
    }


    public class RestauranteCandidato
    {
        public Guid RestauranteId { get; set; }
        public string Nombre { get; set; } = "";
        public string Direccion { get; set; } = "";
        public string ImagenUrl { get; set; } = "";
    }

    public class ResultadoVotacion
    {
        public Guid VotacionId { get; set; }
        public Guid GrupoId { get; set; }
        public EstadoVotacion Estado { get; set; }
        public bool TodosVotaron { get; set; }
        public int MiembrosActivos { get; set; }
        public int TotalVotos { get; set; }

        public List<RestauranteVotado> RestaurantesVotados { get; set; } = new();

        public List<RestauranteCandidato> RestaurantesCandidatos { get; set; } = new();

        public Guid? GanadorId { get; set; }
        public bool HayEmpate { get; set; }
        public List<Guid> RestaurantesEmpatados { get; set; } = new();

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaCierre { get; set; }
    }


    public class RestauranteVotado
    {
        public Guid RestauranteId { get; set; }
        public string RestauranteNombre { get; set; } = "";
        public string RestauranteDireccion { get; set; } = "";
        public string RestauranteImagenUrl { get; set; } = "";
        public int CantidadVotos { get; set; }
        public List<VotanteInfo> Votantes { get; set; } = new();
    }

    public class VotanteInfo
    {
        public Guid UsuarioId { get; set; }
        public string? FirebaseUid { get; set; }
        public string UsuarioNombre { get; set; } = "";
        public string UsuarioFoto { get; set; } = "";
        public string? Comentario { get; set; }
    }
}
