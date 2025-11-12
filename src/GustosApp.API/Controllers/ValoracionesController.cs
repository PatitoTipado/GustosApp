using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValoracionesController : Controller
    {

        private readonly CrearValoracionUsuarioUseCase _crearValoracionUsuarioUseCase;
        private readonly IValoracionUsuarioRepository _valoracionUsuarioRepository;
        private readonly ActualizarValoracionRestauranteUseCase _actualizarValoracionRestauranteUseCase;
        private readonly IMapper _mapper;

        public ValoracionesController(CrearValoracionUsuarioUseCase crearValoracionUsuarioUseCase, IValoracionUsuarioRepository valoracionUsuarioRepository,IMapper  mapper, ActualizarValoracionRestauranteUseCase actualizarValoracionRestauranteUseCase)
        {
            _crearValoracionUsuarioUseCase = crearValoracionUsuarioUseCase;
            _valoracionUsuarioRepository = valoracionUsuarioRepository;
            _mapper = mapper;
            _actualizarValoracionRestauranteUseCase = actualizarValoracionRestauranteUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CrearValoracion([FromBody] CrearValoracionRequest request,CancellationToken ct)
        {
            await _crearValoracionUsuarioUseCase.HandleAsync(
                request.UsuarioId,
                request.RestauranteId,
                request.Valoracion,
                request.Comentario,
                ct
                );

            await _actualizarValoracionRestauranteUseCase.HandleAsync(request.RestauranteId, ct);

            return Ok("Valoracion registrada");
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObtenerValoracionUsuario(Guid usuarioId,CancellationToken ct)
        {
            var valoraciones = await _valoracionUsuarioRepository.ObtenerPorUsuarioAsync(usuarioId, ct);
            var response = _mapper.Map<List<CrearValoracionResponse>>(valoraciones);
            return Ok(valoraciones);
        }
      
    }
}
