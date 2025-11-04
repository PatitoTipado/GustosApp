using System.Security.Claims;
using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CondicionMedicaController : ControllerBase
    {

        private readonly ObtenerCondicionesMedicasUseCase _obtenerCondicionesMed;
        private readonly IMapper _mapper;

        public CondicionMedicaController(ObtenerCondicionesMedicasUseCase obtenerCondicionesMed,
            IMapper mapper)
        {
            _obtenerCondicionesMed = obtenerCondicionesMed;
            _mapper = mapper;
        }

        // GET: api/<ValuesController>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CondicionMedicaResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var (todas, seleccionadas) = await _obtenerCondicionesMed.HandleAsync(uid, ct);

            var response = _mapper.Map<List<CondicionMedicaResponse>>(todas);
            response.ForEach(r => r.Seleccionado = seleccionadas.Contains(r.Id));

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
