using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GustosApp.Application.UseCases; // tu UseCase nuevo o existente
using System.Threading.Tasks;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecomendadorController : ControllerBase
    {
        private readonly ObtenerGustosUseCase _obtenerGustos;
        private readonly SugerirGustosUseCase _sugerirGustos;

        public RecomendadorController(SugerirGustosUseCase sugerirGustos, ObtenerGustosUseCase obtenerGustos)
        {
            _obtenerGustos = obtenerGustos;
            _sugerirGustos = sugerirGustos;
        }

        
       /* [HttpGet("recomendaciones")]
        public async Task<IActionResult> GetRecommendations([FromQuery] int top = 10, CancellationToken ct = default)
        {
            try
            {
                var recommendations = _sugerirGustos.Handle();
                return Ok(new { recommendations });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Token de autenticación inválido." });
            }
        }*/
        

        [HttpGet("recomendaciones")]
        public async Task<IActionResult> GetRecommendations([FromQuery] int top = 10, CancellationToken ct = default)
        {
            var firebaseUid = User.FindFirst("user_id")?.Value
                           ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid)) { 
                return Unauthorized(new { message = "No se encontró el UID de Firebase en el token." });
            }

            var gustos = await _obtenerGustos.Handle(firebaseUid, ct);
            var recommendations = await _sugerirGustos.Handle(gustos, top, ct);
            
            return Ok(new { recommendations });

        }
    }
        

        /*
        [HttpGet("gustos/all")]
        public async Task<IActionResult> GetAllGustos(CancellationToken ct = default)
        {
            var gustos = await _obtenerGustos.HandleAsync(ct);
            return Ok(gustos);
        }*/

    
}