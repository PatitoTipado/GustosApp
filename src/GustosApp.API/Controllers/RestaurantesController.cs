using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
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
        private readonly SugerirGustosUseCase _sugerirGustos;
        private readonly BuscarRestaurantesCercanosUseCase _buscarRestaurantes;
        private readonly ActualizarDetallesRestauranteUseCase _obtenerDetalles;
        private IMapper _mapper;


        public RestaurantesController(IServicioRestaurantes servicio, SugerirGustosUseCase sugerirGustos,
            ObtenerGustosUseCase obtenerGustos,BuscarRestaurantesCercanosUseCase buscarRestaurantes, 
            ActualizarDetallesRestauranteUseCase obtenerDetalles,
            IMapper mapper)
        {
            _servicio = servicio;
            _obtenerGustos = obtenerGustos;
            _sugerirGustos = sugerirGustos;
            _buscarRestaurantes = buscarRestaurantes;
            _obtenerDetalles = obtenerDetalles;
            _mapper = mapper;

        }
        [Authorize]
        [HttpGet("cercanos")]
        public async Task<IActionResult> GetCercanos(double lat, double lng, int radio = 2000, string? types = null, string? priceLevels = null, 
            bool? openNow = null, double? minRating = null, int minUserRatings = 0, string? serves = null, 
            CancellationToken ct = default)
        {

            var result = await _buscarRestaurantes.HandleAsync(lat, lng, radio, types, priceLevels, openNow, minRating, minUserRatings, serves, ct);

            var response = _mapper.Map<List<RestauranteListadoDto>>(result);
            return Ok(new { count = response.Count, restaurantes = response });

          
        }



        [HttpGet("detalles")]
        public async Task<IActionResult> GetDetalles([FromQuery] string placeId, CancellationToken ct = default)
        {
            var restaurante = await _obtenerDetalles.HandleAsync(placeId, ct);

            var detalles = _mapper.Map<RestauranteListadoDto>(restaurante);
            return Ok(new { message = "Detalles actualizados", detalles });
        }

        [HttpGet]
        [Authorize]
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

            var preferencias = await _obtenerGustos.HandleAsync(firebaseUid, ct);

            

            var recommendations = await _sugerirGustos.Handle(preferencias,  top, ct);



            var response = recommendations.Select(r => new RestauranteDto
            {
                Id = r.Id,
                PropietarioUid = r.PropietarioUid,
                Nombre = r.Nombre,
                Direccion = r.Direccion,
                Lat = r.Latitud,
                Lng = r.Longitud,
                ImagenUrl = r.ImagenUrl,
                Rating = r.Rating ?? 0,
                Valoracion = r.Valoracion,
                Tipo = r.Categoria,
                GustosQueSirve = r.GustosQueSirve
             .Select(g => new GustoDto(g.Id, g.Nombre, g.ImagenUrl))
             .ToList(),
                RestriccionesQueRespeta = r.RestriccionesQueRespeta
             .Select(re => new RestriccionResponse(re.Id, re.Nombre))
             .ToList(),
                Score = r.Score
            }).ToList();

            return Ok(new
            {
                total = response.Count,
                recomendaciones = response
            });
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

