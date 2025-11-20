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
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AprobarSolicitudRestauranteUseCase _aprobarSolicitud;
        private readonly ObtenerSolicitudesRestaurantesPendientesUseCase _getPendientes;
        private readonly ObtenerSolicitudRestaurantesPorIdUseCase _getDetalle;
        private readonly IMapper _mapper;

         public AdminController(AprobarSolicitudRestauranteUseCase aprobarSolicitud,
            ObtenerSolicitudesRestaurantesPendientesUseCase getPendientes,
            ObtenerSolicitudRestaurantesPorIdUseCase getDetalle,
            IMapper mapper)
        {
            _aprobarSolicitud = aprobarSolicitud;
            _getPendientes = getPendientes;
            _getDetalle = getDetalle;
            _mapper = mapper;
        }


        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes(CancellationToken ct)
        {
            var result = await _getPendientes.HandleAsync(ct);
            var response = _mapper.Map<SolicitudRestaurantePendienteDto>(result);
            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetalle(Guid id, CancellationToken ct)
        {
            var result = await _getDetalle.HandleAsync(id, ct);
            var response = _mapper.Map<SolicitudRestauranteDetalleDto>(result);

            return result is null ? NotFound() : Ok(response);
        }

        [HttpPost("solicitudes/{id:guid}/aprobar")]
        public async Task<IActionResult> AprobarSolicitud(Guid id, CancellationToken ct)
        {
            var restaurante = await _aprobarSolicitud.HandleAsync(id, ct);


            return Ok(restaurante);

        }


    }
}