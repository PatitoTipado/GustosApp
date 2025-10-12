using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RestriccionController : ControllerBase
    {

        private readonly ObtenerRestriccionesUseCase _obtenerRestricciones;

        public RestriccionController(ObtenerRestriccionesUseCase obtenerRestricciones)
        {
            _obtenerRestricciones = obtenerRestricciones;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException();
            var restricciones = await _obtenerRestricciones.HandleAsync(uid, ct);
            return Ok(new { restricciones });
        }
    }
}
