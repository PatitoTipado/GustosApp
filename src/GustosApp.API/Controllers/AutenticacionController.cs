
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AutenticacionController : ControllerBase
    {
        [HttpGet("quien-soy")]
        [Authorize]
        public IActionResult QuienSoy()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(new { claims });
        }
    }
}
