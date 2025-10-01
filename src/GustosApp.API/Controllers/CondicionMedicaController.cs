using GustosApp.Application.UseCases;
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
            var result = await _obtenerCondicionesMed.HandleAsync(ct);
            return Ok(result);
        }
    }
}
