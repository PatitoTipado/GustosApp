using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GustoController : ControllerBase
    {

        private readonly ObtenerGustosFiltradosUseCase _obtenerGustos;

      
        public GustoController(ObtenerGustosFiltradosUseCase obtenerGustos)
        {
            _obtenerGustos = obtenerGustos;
        }


        // GET: api/<ValuesController>
        
        [HttpGet]
        public async Task<IActionResult> ObtenerGustosFiltrados(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var resp = await _obtenerGustos.HandleAsync(uid, ct);

            // Unir la información
            var gustos = resp.GustosFiltrados
                .Select(g => new GustoDto
                {
                    Id = g.Id,
                    Nombre = g.Nombre,
                    ImagenUrl = g.ImagenUrl,
                    Seleccionado = resp.GustosSeleccionados.Contains(g.Id)
                })
                .ToList();

            return Ok(new
            {
                pasoActual = "Condiciones",
                next = "/registro/gustos",
                gustos
            });

        }
    }
}
