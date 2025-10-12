using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantesController : ControllerBase
    {
        private readonly IServicioRestaurantes _servicio;

        public RestaurantesController(IServicioRestaurantes servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
public async Task<IActionResult> Get(
    [FromQuery(Name = "near.lat")] double? lat,
    [FromQuery(Name = "near.lng")] double? lng,
    [FromQuery(Name = "radiusMeters")] int? radius,
    [FromQuery] string? tipo,
    [FromQuery] string? plato 
)
{
    var res = await _servicio.BuscarAsync(
        tipo: tipo,
        plato: plato,
        lat: lat,
        lng: lng,
        radioMetros: radius
    );
    return Ok(res);
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

