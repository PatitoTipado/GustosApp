using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
        public async Task<IActionResult> GetCercanos(double lat, double lng, int radio = 2000, CancellationToken ct = default)
        {
            var result = await _buscarRestaurantes.HandleAsync(lat, lng, radio, ct);
            return Ok(new
            {
                count = result.Count,
                restaurantes = result
            });
        }


        [HttpGet("detalles")]
        public async Task<IActionResult> GetCercanos(string placeId, CancellationToken ct = default)
        {
            var result = await _obtenerDetalles.HandleAsync(placeId, ct);
            return Ok(new
            {
                message= "Detalles actualizados",
                detalles = result
            });
        }

    }
}
