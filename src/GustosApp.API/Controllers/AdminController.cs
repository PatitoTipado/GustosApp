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
        private readonly ObtenerSolicitudesRestaurantesPendientesUseCase _getPendientes;
        private readonly ObtenerSolicitudRestaurantesPorIdUseCase _getDetalle;
        private readonly RechazarSolicitudRestauranteUseCase _rechazarSolicitud;
        private readonly ObtenerSolicitudesPorTipoUseCase _getPorTipo;
        private readonly IMapper _mapper;
        private readonly IFirebaseAuthService _firebase;
        public AdminController(AprobarSolicitudRestauranteUseCase aprobarSolicitud,
           ObtenerSolicitudesRestaurantesPendientesUseCase getPendientes,
           ObtenerSolicitudRestaurantesPorIdUseCase getDetalle,
           RechazarSolicitudRestauranteUseCase rechazarSolicitud,
           ObtenerSolicitudesPorTipoUseCase getPorTipo,
              IFirebaseAuthService firebase,
            IMapper mapper)
        {
            _aprobarSolicitud = aprobarSolicitud;
            _getPendientes = getPendientes;
            _getDetalle = getDetalle;
            _rechazarSolicitud = rechazarSolicitud;
            _getPorTipo = getPorTipo;
            _firebase = firebase;
            _mapper = mapper;
        }

        /*
        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes(CancellationToken ct)
        {
            
            var result = await _getPendientes.HandleAsync(ct);
            var response = _mapper.Map<List<SolicitudRestaurantePendienteDto>>(result);
            return Ok(response);
        }*/

     [HttpPost("debug/set-admin")]
public async Task<IActionResult> SetAdminDebug()
{
            var firebaseUid = GetFirebaseUid();
            await _firebase.SetUserRoleAsync(firebaseUid, "Admin");
              return Ok("Admin seteado");
}

        
        [HttpGet("me/role")]
        public IActionResult GetMyRole()
        {
            var rol = User.FindFirst("rol")?.Value;
            return Ok(new { rol });
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetalle(Guid id, CancellationToken ct)
        {
            var result = await _getDetalle.HandleAsync(id, ct);
            var response = _mapper.Map<SolicitudRestauranteDetalleDto>(result);

            return result is null ? NotFound() : Ok(response);
        }

        [HttpPost("solicitudes/aprobar/{id:guid}")]
        public async Task<IActionResult> AprobarSolicitud(Guid id, CancellationToken ct)
        {
            var restaurante = await _aprobarSolicitud.HandleAsync(id, ct);

            return Ok(restaurante);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RechazarSolicitud(Guid id,
            [FromBody] string motivoRechazo,
            CancellationToken ct)
        {
            await _rechazarSolicitud.HandleAsync(id, motivoRechazo, ct);
            return Ok(new { message = "Solicitud rechazada correctamente." });
        }

        [HttpGet("solicitudes")]
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