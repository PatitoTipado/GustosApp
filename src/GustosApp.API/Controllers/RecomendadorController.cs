using GustosApp.Application.DTO;
using GustosApp.Application.UseCases; // tu UseCase nuevo o existente
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecomendadorController : ControllerBase
    {
        /*
        private readonly ObtenerGustosUseCase _obtenerGustos;
        private readonly SugerirGustosUseCase _sugerirGustos;

        public RecomendadorController(SugerirGustosUseCase sugerirGustos, ObtenerGustosUseCase obtenerGustos)
        {
            _obtenerGustos = obtenerGustos;
            _sugerirGustos = sugerirGustos;
        }

        [HttpGet("recomendaciones")]
        [AllowAnonymous] // Para evitar usar el token
        public async Task<IActionResult> GetRecommendations([FromQuery] int top = 10, CancellationToken ct = default)
        {
            var firebaseUid = User.FindFirst("user_id")?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
            {
                return Unauthorized(new { message = "No se encontró el UID de Firebase en el token." });
            }

            var preferenciasDTO = await _obtenerGustos.Handle("pjXEEySzWbPqEGkVL1puF7lPvNy2", ct);
            var recommendations = await _sugerirGustos.Handle(preferenciasDTO, top, ct);

            return Ok(new { recommendations });
        }


        [HttpGet("test")]
        [Authorize]
        public IActionResult test()
        {
            return Ok("autorizado");
        }


        [HttpGet("last")]
        [AllowAnonymous] // Para evitar usar el token
        public async Task<IActionResult> last([FromQuery] string uid, CancellationToken ct = default)
        {
            //"pjXEEySzWbPqEGkVL1puF7lPvNy2"
            var preferenciasDTO = await _obtenerGustos.Handle(uid, ct);
            var recommendations = await _sugerirGustos.Handle(preferenciasDTO, 10, ct);

            for (int i = 0; i < preferenciasDTO.Gustos.Count; i++)
            {
                Console.WriteLine(preferenciasDTO.Gustos[i]);
            }

            return Ok(new { recommendations });
        }

        /*[HttpGet("recomendaciones-prueba")]
        [AllowAnonymous] // Para evitar usar el token
        public async Task<ActionResult<List<RecomendacionDTO>>> GetRecommendationsTest([FromQuery] List<string> gustos, [FromQuery] int top = 10,CancellationToken ct = default)
        {
                if (gustos == null || !gustos.Any())
                {
                    return BadRequest(new { message = "Se requiere al menos un gusto para obtener recomendaciones." });
                }
                var recomendaciones = await _sugerirGustos.Handle(gustos, top, ct);

            return Ok(new { recomendaciones });
        }*/
        
    }
}
 