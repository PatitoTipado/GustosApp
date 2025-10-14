using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// Controlador para restaurantes que vienen de la API

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RestauranteController : ControllerBase
    {
        private readonly BuscarRestaurantesCercanosUseCase _buscarRestaurantes;
        private readonly ActualizarDetallesRestauranteUseCase _obtenerDetalles;
        private readonly ObtenerGustosUseCase _obtenerGustos;
        private readonly SugerirGustosUseCase _sugerirGustos;


        public RestauranteController(BuscarRestaurantesCercanosUseCase buscarRestaurantes, ActualizarDetallesRestauranteUseCase obtenerDetalles, SugerirGustosUseCase sugerirGustos, ObtenerGustosUseCase obtenerGustos)
        {
            _buscarRestaurantes = buscarRestaurantes;
            _obtenerDetalles = obtenerDetalles;
        }

        [HttpGet("cercanos")]
        public async Task<IActionResult> GetCercanos([FromQuery] double lat, double lng, int radio = 2000, string? types = null, string? priceLevels = null, bool? openNow = null, double? minRating = null, int minUserRatings = 0, string? serves = null, CancellationToken ct = default, int top = 10)
        {

            var result = await _buscarRestaurantes.HandleAsync(lat, lng, radio, types, priceLevels, openNow, minRating, minUserRatings, serves, ct);

            var firebaseUid = User.FindFirst("user_id")?.Value
               ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
            {
                return Unauthorized(new { message = "No se encontró el UID de Firebase en el token." });
            }

            var preferenciasDTO = await _obtenerGustos.Handle(firebaseUid, ct);
            var recommendations = await _sugerirGustos.Handle(preferenciasDTO, top, ct);

            return Ok(new
            {
                count = result.Count,
                restaurantes = result
            });
        }



        [HttpGet("detalles")]
        public async Task<IActionResult> GetDetalles([FromQuery] string placeId, CancellationToken ct = default)
        {
            var result = await _obtenerDetalles.HandleAsync(placeId, ct);
            return Ok(new
            {
                message = "Detalles actualizados",
                detalles = result
            });
        }


    }
}
