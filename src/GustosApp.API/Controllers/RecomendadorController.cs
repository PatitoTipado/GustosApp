using GustosApp.API.DTO;
using GustosApp.API.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.Services;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GustosApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
   
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
      
        [HttpGet("{restauranteId}/recomendacion")]
        [ProducesResponseType(typeof(RecomendacionResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
 