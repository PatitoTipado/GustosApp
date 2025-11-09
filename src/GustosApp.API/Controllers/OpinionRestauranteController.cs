using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpinionRestauranteController : Controller
    {

        private readonly CrearOpinionRestaurante _crearValoracionUsuarioUseCase;
        private readonly IOpinionRestauranteRepository _opinionRestauranteRepository;
        private readonly ActualizarValoracionRestauranteUseCase _actualizarValoracionRestauranteUseCase;
        private readonly IMapper _mapper;

        public OpinionRestauranteController(CrearOpinionRestaurante crearValoracionUsuarioUseCase, IOpinionRestauranteRepository valoracionUsuarioRepository,IMapper  mapper, ActualizarValoracionRestauranteUseCase actualizarValoracionRestauranteUseCase)
        {
            _crearValoracionUsuarioUseCase = crearValoracionUsuarioUseCase;
            _opinionRestauranteRepository = valoracionUsuarioRepository;
            _mapper = mapper;
            _actualizarValoracionRestauranteUseCase = actualizarValoracionRestauranteUseCase;
        }

        [HttpPost]
        public async Task<IActionResult> CrearValoracion([FromBody] CrearOpinionRestauranteRequest request,CancellationToken ct)
        {
            await _crearValoracionUsuarioUseCase.HandleAsync(
                request.UsuarioId,
                request.RestauranteId,
                request.Valoracion,
                request.Opinion,
                request.Titulo,
                request.Img,
                ct
                );

            await _actualizarValoracionRestauranteUseCase.HandleAsync(request.RestauranteId, ct);

            return Ok("Valoracion registrada");
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> ObtenerValoracionUsuario(Guid usuarioId,CancellationToken ct)
        {
            var valoraciones = await _opinionRestauranteRepository.ObtenerPorUsuarioAsync(usuarioId, ct);
            var response = _mapper.Map<List<CrearOpinionRestauranteResponse>>(valoraciones);
            return Ok(valoraciones);
        }
      
    }
}
