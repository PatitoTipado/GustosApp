using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes;
using GustosApp.Domain.Common;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OpinionRestauranteController : BaseApiController
    {

        private readonly CrearOpinionRestauranteUseCase _crearValoracionUsuarioUseCase;
        private readonly IOpinionRestauranteRepository _opinionRestauranteRepository;
        private readonly ActualizarValoracionRestauranteUseCase _actualizarValoracionRestauranteUseCase;
        private readonly IMapper _mapper;
        private readonly IServicioRestaurantes _servicioRestaurantes;
        private readonly ObtenerValoracionUseCase _obtenerValoracionUseCase;

        public OpinionRestauranteController(CrearOpinionRestauranteUseCase crearValoracionUsuarioUseCase, IOpinionRestauranteRepository valoracionUsuarioRepository,IMapper  mapper, ActualizarValoracionRestauranteUseCase actualizarValoracionRestauranteUseCase, IServicioRestaurantes servicioRestaurantes,ObtenerValoracionUseCase obtenerValoracionUseCase)
        {
            _crearValoracionUsuarioUseCase = crearValoracionUsuarioUseCase;
            _opinionRestauranteRepository = valoracionUsuarioRepository;
            _mapper = mapper;
            _actualizarValoracionRestauranteUseCase = actualizarValoracionRestauranteUseCase;
            _servicioRestaurantes = servicioRestaurantes;
            _obtenerValoracionUseCase = obtenerValoracionUseCase;  
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CrearOpinion([FromForm] CrearOpinionRestauranteRequest request,CancellationToken ct)
        {

            var uid= GetFirebaseUid();

            var files = request.Imagenes?.Select(img => new FileUpload
            {
                FileName = img.FileName,
                Content = img.OpenReadStream(),
                ContentType = img.ContentType
            }).ToList();

            await _crearValoracionUsuarioUseCase.HandleAsync(
                uid,
                request.RestauranteId,
                request.Valoracion,
                request.Opinion,
                request.Titulo,
                files,
                request.MotivoVisita,
                request.FechaVisita ?? DateTime.Now,
                ct
                );

            await _actualizarValoracionRestauranteUseCase.HandleAsync(request.RestauranteId, ct);

            return Ok("Opinion registrada");
        }

        [HttpGet("{usuarioId}")]
        [ProducesResponseType(typeof(List<CrearOpinionRestauranteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerValoracionUsuario(Guid usuarioId, CancellationToken ct)
        {
            var valoraciones = await _opinionRestauranteRepository.ObtenerPorUsuarioAsync(usuarioId, ct);
            var response = _mapper.Map<List<CrearOpinionRestauranteResponse>>(valoraciones);
            return Ok(response);
        }

        [HttpGet("google-metrica/{placeId}")]
        [ProducesResponseType(typeof(GooglePlacesDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerMetricasGooglePrueba(string placeId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(placeId))
            {
                return BadRequest("El PlaceId no puede estar vacío.");
            }

            // El método devuelve el Rating y TotalRatings,primero buscando en Google Places y si no encuentra, busca en la base de datos como fallback.
            var metrics = await _servicioRestaurantes.ObtenerMetricasGooglePlaces(placeId, ct);

            if (metrics == null)
            {
                return NotFound($"No se encontraron metricas para el PlaceId: {placeId}. Verifica que el Place ID sea valido. Revisa los logs del servidor para más detalles.");
            }

            var response = new GooglePlacesDto
            {
                Rating = metrics.Rating,
                TotalRatings = metrics.TotalRatings
            };

            return Ok(response);
        }

        [HttpGet("valoracion/{restauranteId}")]
        [ProducesResponseType(typeof(ValoracionCombinadaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObtenerValoracionCombinada(Guid restauranteId, CancellationToken ct)
        {
            try
            {
                var result = await _obtenerValoracionUseCase.HandleAsync(restauranteId, ct);

                var response = new ValoracionCombinadaResponse
                {
                    RestauranteId = result.RestauranteId,
                    NombreRestaurante = result.NombreRestaurante,
                    ValoracionCombinada = result.ValoracionCombinada,
                    TotalValoraciones = result.TotalValoraciones,
                    ValoracionUsuariosApp = new ValoracionFuente
                    {
                        Rating = result.ValoracionUsuariosApp.Rating,
                        CantidadValoraciones = result.ValoracionUsuariosApp.CantidadValoraciones,
                        Disponible = result.ValoracionUsuariosApp.Disponible
                    },
                    ValoracionGooglePlaces = new ValoracionFuente
                    {
                        Rating = result.ValoracionGooglePlaces.Rating,
                        CantidadValoraciones = result.ValoracionGooglePlaces.CantidadValoraciones,
                        Disponible = result.ValoracionGooglePlaces.Disponible
                    }
                };
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener valoración combinada: {ex.Message}");
            }
        }

    }
}
