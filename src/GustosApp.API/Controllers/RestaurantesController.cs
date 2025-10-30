using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Services;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// Controlador para restaurantes que se registran en la app por un usuario

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantesController : ControllerBase
    {
        private readonly IServicioRestaurantes _servicio;
        private readonly ObtenerGustosUseCase _obtenerGustos;
        private readonly SugerirGustosSobreUnRadioUseCase _sugerirGustos;
        private readonly BuscarRestaurantesCercanosUseCase _buscarRestaurantes;
        private readonly ActualizarDetallesRestauranteUseCase _obtenerDetalles;


        public RestaurantesController(IServicioRestaurantes servicio, SugerirGustosSobreUnRadioUseCase sugerirGustos,
            ObtenerGustosUseCase obtenerGustos,BuscarRestaurantesCercanosUseCase buscarRestaurantes, 
            ActualizarDetallesRestauranteUseCase obtenerDetalles)
        {
            _servicio = servicio;
            _obtenerGustos = obtenerGustos;
            _sugerirGustos = sugerirGustos;
            _buscarRestaurantes = buscarRestaurantes;
            _obtenerDetalles = obtenerDetalles;

        }
        [HttpGet("cercanos")]
        public async Task<IActionResult> GetCercanos(double lat, double lng, int radio = 2000, string? types = null, string? priceLevels = null, 
            bool? openNow = null, double? minRating = null, int minUserRatings = 0, string? serves = null, 
            CancellationToken ct = default)
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

        [HttpGet]
        public async Task<IActionResult> Get(
            CancellationToken ct,
            [FromQuery(Name = "near.lat")] double? lat = -34.641812775271,
            [FromQuery(Name = "near.lng")] double? lng = -58.56990230458638,
            [FromQuery(Name = "radiusMeters")] int? radius = 1000,
            [FromQuery] string? tipo = "",
            [FromQuery] string? plato = "",
            [FromQuery] int top = 10
        )
        {
            var res = await _servicio.BuscarAsync(
                tipo: tipo,
                plato: plato,
                lat: lat,
                lng: lng,
                radioMetros: radius
            );

            Console.WriteLine(res.ToList().Count());
            Console.WriteLine(res.ToList().Count());
            Console.WriteLine(res.ToList().Count());


            var firebaseUid = User.FindFirst("user_id")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
            {
                return Unauthorized(new { message = "No se encontró el UID de Firebase en el token." });
            }

            var preferenciasDTO = await _obtenerGustos.Handle(firebaseUid, ct);

            Console.WriteLine(preferenciasDTO.ToString());
            Console.WriteLine(preferenciasDTO.ToString());
            Console.WriteLine(preferenciasDTO.ToString());

            var recommendations = _sugerirGustos.Handle(preferenciasDTO, res.ToList(), top, ct);

            Console.WriteLine(recommendations.ToList().Count());
            Console.WriteLine(recommendations.ToList().Count());
            Console.WriteLine(recommendations.ToList().Count());

            foreach (var item in recommendations)
            {
                foreach (var gusto in item.GustosQueSirve)
                {
                    Console.WriteLine(gusto.Nombre);
                }
            }

            return Ok(recommendations);
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var r = await _servicio.ObtenerAsync(id);
            return r is null ? NotFound() : Ok(r);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CrearRestauranteDto dto)
        {
            var uid = User.FindFirst("user_id")?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();

            var creado = await _servicio.CrearAsync(uid, dto);
            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] ActualizarRestauranteDto dto)
        {
            var uid = User.FindFirst("user_id")?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();

            var esAdmin = User.IsInRole("admin") || User.Claims.Any(c => c.Type == "role" && c.Value == "admin");

            var r = await _servicio.ActualizarAsync(id, uid, esAdmin, dto);
            return r is null ? NotFound() : Ok(r);
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var uid = User.FindFirst("user_id")?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();

            var esAdmin = User.IsInRole("admin") || User.Claims.Any(c => c.Type == "role" && c.Value == "admin");

            var ok = await _servicio.EliminarAsync(id, uid, esAdmin);
            return ok ? NoContent() : NotFound();
        }
    }
}

