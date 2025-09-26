using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GustoController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        public string Hola()
        {
            return "Hola soy GustoApp";
        }
    }
}
