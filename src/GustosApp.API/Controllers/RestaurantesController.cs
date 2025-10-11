
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/restaurantes")]
    [Authorize]
    public class RestaurantesController : ControllerBase
    {
        private readonly IServicioRestaurantes _servicio;

        public RestaurantesController(IServicioRestaurantes servicio)
        {
            _servicio = servicio;
        }

        private string? ObtenerUid() => User.FindFirstValue("user_id") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        private bool EsAdmin() => string.Equals(User.FindFirst("role")?.Value, "admin", StringComparison.OrdinalIgnoreCase);

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearRestauranteDto dto)
        {
            try
            {
                var uid = ObtenerUid();
                if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();
                var creado = await _servicio.CrearAsync(uid, dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObtenerPorId([FromRoute] Guid id)
        {
            var r = await _servicio.ObtenerAsync(id);
            return r is null ? NotFound() : Ok(r);
        }

        [HttpGet("mio")]
        public async Task<IActionResult> ObtenerMio()
        {
            var uid = ObtenerUid();
            if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();
            var r = await _servicio.ObtenerPorPropietarioAsync(uid);
            return r is null ? NotFound() : Ok(r);
        }

        [HttpGet]
        public async Task<IActionResult> ListarCercanos([FromQuery(Name = "near.lat")] double lat,
                                                        [FromQuery(Name = "near.lng")] double lng,
                                                        [FromQuery] int radiusMeters = 3000)
        {
            var lista = await _servicio.ListarCercanosAsync(lat, lng, radiusMeters <= 0 ? 3000 : radiusMeters);
            return Ok(lista);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Actualizar([FromRoute] Guid id, [FromBody] ActualizarRestauranteDto dto)
        {
            try
            {
                var uid = ObtenerUid();
                if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();
                var actualizado = await _servicio.ActualizarAsync(id, uid, EsAdmin(), dto);
                return actualizado is null ? NotFound() : Ok(actualizado);
            }
            catch (ArgumentException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Eliminar([FromRoute] Guid id)
        {
            try
            {
                var uid = ObtenerUid();
                if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();
                var ok = await _servicio.EliminarAsync(id, uid, EsAdmin());
                return ok ? NoContent() : NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
