using AutoMapper;
using GustosApp.API.DTO;
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
        private IMapper _mapper;


        public GustoController(ObtenerGustosFiltradosUseCase obtenerGustos,
            IMapper mapper)
        {
            _obtenerGustos = obtenerGustos;
            _mapper = mapper;
        }


        // GET: api/<ValuesController>

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

        private string GetFirebaseUid()
        {
            var firebaseUid = User.FindFirst("user_id")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                throw new UnauthorizedAccessException("No se encontró el UID de Firebase en el token.");

            return firebaseUid;
        }
    }
}
