using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GustoController : ControllerBase
    {

        private readonly ObtenerGustosUseCase _obtenerGustos;

      
        public GustoController(ObtenerGustosUseCase obtenerGustos)
        {
            _obtenerGustos = obtenerGustos;
        }


        // GET: api/<ValuesController>
       
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _obtenerGustos.HandleAsync(ct);
            return Ok(result);
        }
    }
}
