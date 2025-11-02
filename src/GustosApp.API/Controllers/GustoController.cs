using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GustoController : ControllerBase
    {

        private readonly ObtenerGustosFiltradosUseCase _obtenerGustos;
        private readonly ObtenerGustosPaginacionUseCase _obtenerGustosPaginacion;
        private readonly BuscarGustoPorCoincidenciaUseCase _buscarGustoPorCoincidencia;
        private readonly ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase _obtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase;

        public GustoController(
            ObtenerGustosFiltradosUseCase obtenerGustos,
            ObtenerGustosPaginacionUseCase obtenerGustosPaginacion,
            BuscarGustoPorCoincidenciaUseCase buscarGustoPorCoincidenciaUseCase,
            ObtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase obtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase)
        {
            _obtenerGustos = obtenerGustos;
            _obtenerGustosPaginacion = obtenerGustosPaginacion;
            _buscarGustoPorCoincidencia = buscarGustoPorCoincidenciaUseCase;
            _obtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase = obtenerGustosSeleccionadosPorUsuarioYParaFiltrarUseCase;
        }
        private string GetFirebaseUid()
        {
            var firebaseUid = User.FindFirst("user_id")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                throw new UnauthorizedAccessException("No se encontró el UID de Firebase en el token.");

            return firebaseUid;
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

        [HttpGet]
        public async Task<IActionResult> ObtenerGustosFiltrados(CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var resp = await _obtenerGustos.HandleAsync(uid, ct);

            // Unir la información
            var gustos = resp.GustosFiltrados
                .Select(g => new GustoDto
                {
                    Id = g.Id,
                    Nombre = g.Nombre,
                    ImagenUrl = g.ImagenUrl,
                    Seleccionado = resp.GustosSeleccionados.Contains(g.Id)
                })
                .ToList();

            return Ok(new
            {
                pasoActual = "Condiciones",
                next = "/registro/gustos",
                gustos
            });

        }
    }
}
