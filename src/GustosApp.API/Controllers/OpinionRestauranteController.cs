using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases.OpinionesRestaurantes;
using GustosApp.Domain.Common;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OpinionRestauranteController : BaseApiController
    {

        private readonly CrearOpinionRestauranteUseCase _crearValoracionUsuarioUseCase;
        private readonly IOpinionRestauranteRepository _opinionRestauranteRepository;
        private readonly ActualizarValoracionRestauranteUseCase _actualizarValoracionRestauranteUseCase;
        private readonly IMapper _mapper;

        public OpinionRestauranteController(CrearOpinionRestauranteUseCase crearValoracionUsuarioUseCase, IOpinionRestauranteRepository valoracionUsuarioRepository,IMapper  mapper, ActualizarValoracionRestauranteUseCase actualizarValoracionRestauranteUseCase)
        {
            _crearValoracionUsuarioUseCase = crearValoracionUsuarioUseCase;
            _opinionRestauranteRepository = valoracionUsuarioRepository;
            _mapper = mapper;
            _actualizarValoracionRestauranteUseCase = actualizarValoracionRestauranteUseCase;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CrearValoracion([FromForm] CrearOpinionRestauranteRequest request,CancellationToken ct)
        {

            var uid= GetFirebaseUid();

            var files = request.Imagenes?.Select(img => new FileUpload
            {
                FileName = img.FileName,
                Content = img.OpenReadStream(),
                ContentType = img.ContentType
            }).ToList();

            await _crearValoracionUsuarioUseCase.HandleAsync(
                uid,
                request.RestauranteId,
                request.Valoracion,
                request.Opinion,
                request.Titulo,
                files,
                request.MotivoVisita,
                request.FechaVisita ?? DateTime.Now,
                ct
                );

            await _actualizarValoracionRestauranteUseCase.HandleAsync(request.RestauranteId, ct);

            return Ok("Valoracion registrada");
        }

        [HttpGet("{usuarioId}")]
        [ProducesResponseType(typeof(List<CrearOpinionRestauranteResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerValoracionUsuario(Guid usuarioId,CancellationToken ct)
        {
            var valoraciones = await _opinionRestauranteRepository.ObtenerPorUsuarioAsync(usuarioId, ct);
            var response = _mapper.Map<List<CrearOpinionRestauranteResponse>>(valoraciones);
            return Ok(valoraciones);
        }
      
    }
}
