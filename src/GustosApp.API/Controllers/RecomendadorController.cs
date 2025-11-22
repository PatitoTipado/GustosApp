using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.Services;
using GustosApp.Application.UseCases; // tu UseCase nuevo o existente
using GustosApp.Domain.Interfaces;
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
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IRestauranteRepository _restauranteRepository;
        private readonly IRecomendacionAIService _recomendacionAIService;
        private readonly RecomendacionIAUseCase _recomendacionIAUseCase;

        public RecomendadorController(IUsuarioRepository usuarioRepository,IRestauranteRepository restauranteRepository,IRecomendacionAIService recomendacionAIService, RecomendacionIAUseCase recomendacionIAUseCase)
        {
            _usuarioRepository = usuarioRepository;
            _restauranteRepository = restauranteRepository;
            _recomendacionAIService = recomendacionAIService;
            _recomendacionIAUseCase = recomendacionIAUseCase;

        }
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

        [HttpGet("{restauranteId}")]
        public async Task<IActionResult> ObtenerRecomendacion(Guid restauranteId, CancellationToken ct)
        {
            // 1) Obtener UID del usuario autenticado
            var usuarioId = User.FindFirst("user_id")?.Value;

            if (usuarioId == null)
                return BadRequest("No se pudo obtener el usuario autenticado.");

            // 2) Obtener datos del usuario y del restaurante desde 
            var usuario = await _usuarioRepository.GetByFirebaseUidAsync(usuarioId);

            var restaurante = await _restauranteRepository.GetByIdAsync(restauranteId,ct);

            if (usuario == null || restaurante == null)
                return NotFound();

            // 3) Armar prompt
            var explicacion = await _recomendacionIAUseCase.Handle(usuario, restaurante,ct);

            return Ok(new RecomendacionResponse
            {
                RestauranteId = restauranteId,
                Explicacion = explicacion
            });
        }


    }
}
 