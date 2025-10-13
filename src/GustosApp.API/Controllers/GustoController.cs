using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GustoController : ControllerBase
    {

        private readonly ObtenerGustosUseCase _obtenerGustos;

      
        public GustoController(ObtenerGustosUseCase obtenerGustos)
        {
            _obtenerGustos = obtenerGustos;
        }


        // GET: api/<ValuesController>
       
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var firebaseUid = User.FindFirst("user_id")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                return Unauthorized(new { message = "No se encontró el UID de Firebase en el token." });

            var result = await _obtenerGustos.Handle(firebaseUid,ct);
            return Ok(result);
        }
    }
}
