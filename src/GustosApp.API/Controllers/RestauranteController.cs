using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

// Controlador para restaurantes que vienen de la API

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RestauranteController : ControllerBase
    {
        private readonly BuscarRestaurantesCercanosUseCase _buscarRestaurantes;
        private readonly ActualizarDetallesRestauranteUseCase _obtenerDetalles;


        public RestauranteController(BuscarRestaurantesCercanosUseCase buscarRestaurantes, ActualizarDetallesRestauranteUseCase obtenerDetalles)
        {
            _buscarRestaurantes = buscarRestaurantes;
            _obtenerDetalles = obtenerDetalles;
        }

        [HttpGet("cercanos")]
        public async Task<IActionResult> GetCercanos(double lat, double lng, int radio = 2000, string? types = null, string? priceLevels = null, bool? openNow = null, double? minRating = null, int minUserRatings = 0, string? serves = null, CancellationToken ct = default)
        {

            var result = await _buscarRestaurantes.HandleAsync(lat, lng, radio, types, priceLevels, openNow, minRating, minUserRatings, serves, ct);


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
