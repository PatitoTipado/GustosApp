using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;
using GustosApp.Domain.Model.@enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GustoController : BaseApiController
    {
        private readonly GuardarGustosUseCase _saveGustos;
        private readonly ObtenerGustosFiltradosUseCase _obtenerGustos;
        private readonly ObtenerGustosPaginacionUseCase _obtenerGustosPaginacion;
        private readonly BuscarGustoPorCoincidenciaUseCase _buscarGustoPorCoincidencia;
        private readonly ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase _obtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase;
       

        private readonly IMapper _mapper;
        public GustoController(
           GuardarGustosUseCase saveGustos,
           ObtenerGustosFiltradosUseCase obtenerGustos,
            IMapper mapper,
             ObtenerGustosPaginacionUseCase obtenerGustosPaginacion,
            BuscarGustoPorCoincidenciaUseCase buscarGustoPorCoincidenciaUseCase,
            ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase obtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase)
        {
            _saveGustos = saveGustos;
            _obtenerGustos = obtenerGustos;
            _obtenerGustosPaginacion = obtenerGustosPaginacion;
            _buscarGustoPorCoincidencia = buscarGustoPorCoincidenciaUseCase;
            _obtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase = obtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase;  
            _mapper = mapper;
            
        }
        

        [HttpGet("obtenerGustosPaginados")]
        public async Task<IActionResult> ObtenerGustos(CancellationToken ct, int cantidadInicio,int final)
        {
            var uid = GetFirebaseUid();

            var resp = await _obtenerGustosPaginacion.HandleAsync(cantidadInicio,final);

            return Ok(resp);
        }

        [HttpGet("buscarGustoPorCoincidencia")]
        public async Task<IActionResult> buscarGustoPorCoincidencia(CancellationToken ct, string gustoNombre)
        {
            var uid = GetFirebaseUid();

            var resp = await _buscarGustoPorCoincidencia.HandleAsync(gustoNombre);

            return Ok(resp);
        }

        [HttpGet("obtenerGustoFiltros")]
        public async Task<IActionResult> obtenerGustoFiltros(CancellationToken ct,int inicio,int final)
        {
            var uid = GetFirebaseUid();

            var resp = await _obtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase.HandleAsync(uid,inicio,final);

            return Ok(resp);
        }

        

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ObtenerGustosFiltradosResponse),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerGustosFiltrados(CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var result = await _obtenerGustos.HandleAsync(uid, ct);

            var gustos = _mapper.Map<List<GustoDto>>(result.GustosFiltrados);

            foreach (var g in gustos)
                g.Seleccionado = result.GustosSeleccionados.Contains(g.Id);

            var response = new ObtenerGustosFiltradosResponse
            {
                PasoActual = "Condiciones",
                Next = "/registro/gustos",
                Gustos = gustos
            };

            return Ok(response);
        }





        [Authorize]
        [HttpPut("gustos")]
        [ProducesResponseType(typeof(GuardarGustosResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GuardarGustos([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var gustos = await _saveGustos.HandleAsync(uid, req.Ids, ModoPreferencias.Edicion, ct);

            var response = new GuardarGustosResponse
            {
                Mensaje = "Gustos guardados correctamente",
                GustosIncompatibles = gustos
            };
            return Ok(response);
        }
    }
}
