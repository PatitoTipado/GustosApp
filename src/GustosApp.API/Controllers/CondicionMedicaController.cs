using System.Security.Claims;
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

        public CondicionMedicaController(ObtenerCondicionesMedicasUseCase obtenerCondicionesMed)
        {
            _obtenerCondicionesMed = obtenerCondicionesMed;
        }

        // GET: api/<ValuesController>
        
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var result = await _obtenerCondicionesMed.HandleAsync(uid,ct);
            return Ok(result);
        }
    }
}
