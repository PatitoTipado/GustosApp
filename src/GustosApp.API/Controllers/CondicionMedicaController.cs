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
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException();

            var result = await _obtenerCondicionesMed.HandleAsync(uid,ct);
            return Ok(result);
        }
    }
}
