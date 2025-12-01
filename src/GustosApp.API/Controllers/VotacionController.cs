using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.API.DTO;
using GustosApp.Application.UseCases.VotacionUseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class VotacionController : BaseApiController
    {
        private readonly IniciarVotacionUseCase _iniciarVotacionUseCase;
        private readonly RegistrarVotoUseCase _registrarVotoUseCase;
        private readonly ObtenerResultadosVotacionUseCase _obtenerResultadosUseCase;
        private readonly CerrarVotacionUseCase _cerrarVotacionUseCase;
        private readonly SeleccionarGanadorRuletaUseCase _seleccionarGanadorRuletaUseCase;
        private readonly IVotacionRepository _votacionRepository;

        public VotacionController(
            IniciarVotacionUseCase iniciarVotacionUseCase,
            RegistrarVotoUseCase registrarVotoUseCase,
            ObtenerResultadosVotacionUseCase obtenerResultadosUseCase,
            CerrarVotacionUseCase cerrarVotacionUseCase,
            SeleccionarGanadorRuletaUseCase seleccionarGanadorRuletaUseCase,
            IVotacionRepository votacionRepository)
        {
            _iniciarVotacionUseCase = iniciarVotacionUseCase;
            _registrarVotoUseCase = registrarVotoUseCase;
            _obtenerResultadosUseCase = obtenerResultadosUseCase;
            _cerrarVotacionUseCase = cerrarVotacionUseCase;
            _seleccionarGanadorRuletaUseCase = seleccionarGanadorRuletaUseCase;
            _votacionRepository = votacionRepository;
        }

        [HttpPost("iniciar")]
        [ProducesResponseType(typeof(VotacionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> IniciarVotacion(
            [FromBody] IniciarVotacionRequest request,
            CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();
            var votacion = await _iniciarVotacionUseCase.HandleAsync(
                firebaseUid,
                request.GrupoId,
                request.Descripcion,
                request.RestaurantesCandidatos,
                ct
            );



            var response = new VotacionResponse
            {
                Id = votacion.Id,
                GrupoId = votacion.GrupoId,
                Estado = votacion.Estado.ToString(),
                FechaInicio = votacion.FechaInicio,
                Descripcion = votacion.Descripcion
            };

            return Ok(response);
        }

        [HttpPost("{votacionId}/votar")]
        [ProducesResponseType(typeof(VotoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegistrarVoto(
            Guid votacionId,
            [FromBody] RegistrarVotoRequest request,
            CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();
            var voto = await _registrarVotoUseCase.HandleAsync(
                firebaseUid,
                votacionId,
                request.RestauranteId,
                request.Comentario,
                ct);

            var response = new VotoResponse
            {
                Id = voto.Id,
                VotacionId = voto.VotacionId,
                RestauranteId = voto.RestauranteId,
                FechaVoto = voto.FechaVoto,
                Comentario = voto.Comentario
            };

            return Ok(response);
        }

        [HttpGet("grupo/{grupoId}/activa")]
        [ProducesResponseType(typeof(ResultadoVotacionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerVotacionActiva(
            Guid grupoId,
            CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();

            // Primero obtenemos la votación activa del grupo
            var votacionActiva = await _votacionRepository.ObtenerVotacionActivaAsync(grupoId, ct);

            if (votacionActiva == null)
                return NotFound(new { message = "No hay votación activa en este grupo" });

            // Luego obtenemos los resultados de esa votación
            var resultado = await _obtenerResultadosUseCase.HandleAsync(firebaseUid, votacionActiva.Id, ct);

            var response = MapToResponse(resultado);
            return Ok(response);
        }

        [HttpGet("{votacionId}/resultados")]
        [ProducesResponseType(typeof(ResultadoVotacionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerResultados(
            Guid votacionId,
            CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();
            var resultado = await _obtenerResultadosUseCase.HandleAsync(firebaseUid, votacionId, ct);

            var response = MapToResponse(resultado);
            return Ok(response);
        }

        [HttpPost("{votacionId}/cerrar")]
        [ProducesResponseType(typeof(VotacionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CerrarVotacion(
         Guid votacionId,
         [FromBody] CerrarVotacionRequest? request = null,
         CancellationToken ct = default)
        {
            var firebaseUid = GetFirebaseUid();
            var votacion = await _cerrarVotacionUseCase.HandleAsync(
                firebaseUid,
                votacionId,
                request?.RestauranteGanadorId,
                ct);

            var response = new VotacionResponse
            {
                Id = votacion.Id,
                GrupoId = votacion.GrupoId,
                Estado = votacion.Estado.ToString(),
                FechaInicio = votacion.FechaInicio,
                FechaCierre = votacion.FechaCierre,
                RestauranteGanadorId = votacion.RestauranteGanadorId,
                Descripcion = votacion.Descripcion
            };

            return Ok(response);
        }

        [HttpPost("{votacionId}/seleccionar-ganador")]
        [ProducesResponseType(typeof(VotacionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SeleccionarGanadorRuleta(
            Guid votacionId,
            [FromBody] SeleccionarGanadorRequest request,
            CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();
            var votacion = await _seleccionarGanadorRuletaUseCase.HandleAsync(
                firebaseUid,
                votacionId,
                request.RestauranteGanadorId,
                ct);

            var response = new VotacionResponse
            {
                Id = votacion.Id,
                GrupoId = votacion.GrupoId,
                Estado = votacion.Estado.ToString(),
                FechaInicio = votacion.FechaInicio,
                FechaCierre = votacion.FechaCierre,
                RestauranteGanadorId = votacion.RestauranteGanadorId,
                Descripcion = votacion.Descripcion
            };

            return Ok(response);
        }

        private ResultadoVotacionResponse MapToResponse(ResultadoVotacion resultado)
        {
            return new ResultadoVotacionResponse
            {
                VotacionId = resultado.VotacionId,
                GrupoId = resultado.GrupoId,
                Estado = resultado.Estado.ToString(),
                TodosVotaron = resultado.TodosVotaron,
                MiembrosActivos = resultado.MiembrosActivos,
                TotalVotos = resultado.TotalVotos,

                RestaurantesVotados = resultado.RestaurantesVotados.Select(r => new RestauranteVotadoDto
                {
                    RestauranteId = r.RestauranteId,
                    RestauranteNombre = r.RestauranteNombre,
                    RestauranteDireccion = r.RestauranteDireccion,
                    RestauranteImagenUrl = r.RestauranteImagenUrl,
                    CantidadVotos = r.CantidadVotos,
                    Votantes = r.Votantes.Select(v => new VotanteInfoDto
                    {
                        UsuarioId = v.UsuarioId,
                        UsuarioNombre = v.UsuarioNombre,
                        UsuarioFoto = v.UsuarioFoto,
                        Comentario = v.Comentario
                    }).ToList()
                }).ToList(),

                RestaurantesCandidatos = resultado.RestaurantesCandidatos
                    .Select(rc => new RestauranteCandidatoDto
                    {
                        RestauranteId = rc.RestauranteId,
                        Nombre = rc.Nombre,
                        Direccion = rc.Direccion,
                        ImagenUrl = rc.ImagenUrl
                    })
                    .ToList(),

                GanadorId = resultado.GanadorId,
                HayEmpate = resultado.HayEmpate,
                RestaurantesEmpatados = resultado.RestaurantesEmpatados,
                FechaInicio = resultado.FechaInicio,
                FechaCierre = resultado.FechaCierre
            };
        }
    }
    }
