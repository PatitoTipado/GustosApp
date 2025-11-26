using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases;
using GustosApp.Application.UseCases.AmistadUseCases;
using AutoMapper;
using GustosApp.API.DTO;

namespace GustosApp.API.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("[controller]")]
    [ApiController]

    public class AdminController : BaseApiController
    {
        private readonly AprobarSolicitudRestauranteUseCase _aprobarSolicitud;
        private readonly ObtenerSolicitudRestaurantesPorIdUseCase _getDetalle;
        private readonly RechazarSolicitudRestauranteUseCase _rechazarSolicitud;
        private readonly ObtenerSolicitudesPorTipoUseCase _getPorTipo;
        private readonly IMapper _mapper;
       
        public AdminController(AprobarSolicitudRestauranteUseCase aprobarSolicitud,
           ObtenerSolicitudRestaurantesPorIdUseCase getDetalle,
           RechazarSolicitudRestauranteUseCase rechazarSolicitud,
           ObtenerSolicitudesPorTipoUseCase getPorTipo,
            IMapper mapper)
        {
            _aprobarSolicitud = aprobarSolicitud;
            _getDetalle = getDetalle;
            _rechazarSolicitud = rechazarSolicitud;
            _getPorTipo = getPorTipo;
            _mapper = mapper;
        }

 
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(SolicitudRestauranteDetalleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDetalle(Guid id, CancellationToken ct)
        {
            var result = await _getDetalle.HandleAsync(id, ct);
            var response = _mapper.Map<SolicitudRestauranteDetalleDto>(result);

            return result is null ? NotFound() : Ok(response);
        }

        [HttpPost("solicitudes/aprobar/{id:guid}")]
        [ProducesResponseType(typeof(Restaurante), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> AprobarSolicitud(Guid id, CancellationToken ct)
        {
            var restaurante = await _aprobarSolicitud.HandleAsync(id, ct);

            return Ok(restaurante);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RechazarSolicitud(Guid id,
           [FromBody] string motivoRechazo,
           CancellationToken ct)
        {
            await _rechazarSolicitud.HandleAsync(id, motivoRechazo, ct);
            return Ok(new { message = "Solicitud rechazada correctamente." });
        }

        [HttpGet("solicitudes")]
        [ProducesResponseType(typeof(SolicitudRestaurantePendienteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPorTipo(
         [FromQuery] EstadoSolicitudRestaurante tipo = EstadoSolicitudRestaurante.Pendiente,
         CancellationToken ct = default)
        {
            var result = await _getPorTipo.HandleAsync(tipo, ct);

            var response = _mapper.Map<List<SolicitudRestaurantePendienteDto>>(result);

            return Ok(response);
        }


    }
}