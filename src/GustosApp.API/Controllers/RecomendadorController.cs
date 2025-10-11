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

            if (string.IsNullOrWhiteSpace(firebaseUid))
            {
                return Unauthorized(new { message = "No se encontró el UID de Firebase en el token." });
            }

            var gustos = await _obtenerGustos.Handle(firebaseUid, ct);
            var recommendations = await _sugerirGustos.Handle(gustos, top, ct);

            return Ok(new { recommendations });

        }


        [HttpGet("recomendaciones-prueba")]
        [AllowAnonymous] // Para evitar usar el token
        public async Task<ActionResult<List<RecomendacionDTO>>> GetRecommendationsTest([FromQuery] int top = 10, CancellationToken ct = default)
        {
            try
            {
                var gustosUsuario = new List<string> { "Pizza","Sushi" };
                var recomendaciones = await _sugerirGustos.Handle(gustosUsuario, top, ct);
                var resultadoFinal = recomendaciones.Select(r => new
                {
                    RestaurantId = r.RestaurantId,
                    Score = r.Score,
                }).ToList();

                return Ok(resultadoFinal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Ocurrió un error al procesar las recomendaciones: {ex.Message}" });
            }
        }
    }
      

    }