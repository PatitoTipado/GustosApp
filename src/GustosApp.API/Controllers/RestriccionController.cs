using System.Security.Claims;
using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RestriccionController : ControllerBase
    {

        private readonly ObtenerRestriccionesUseCase _obtenerRestricciones;
        private readonly ObtenerUsuarioUseCase _usuario; 
        private readonly IMapper _mapper;

        public RestriccionController(ObtenerRestriccionesUseCase obtenerRestricciones
            ,ObtenerUsuarioUseCase usuario, IMapper mapper)
        {
            _obtenerRestricciones = obtenerRestricciones;
            _usuario = usuario;
            _mapper = mapper;
        }

        // GET: api/<ValuesController>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(List<RestriccionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });


            var usuario = await _usuario.HandleAsync(uid, ct)
                          ?? throw new InvalidOperationException("Usuario no encontrado.");

            var todas = await _obtenerRestricciones.HandleAsync(uid,ct);
            var seleccionadas = usuario.Restricciones.Select(r => r.Id).ToHashSet();

       
            var resultado = todas.Select(r =>
            {
                var dto = _mapper.Map<RestriccionDto>(r);
                dto.Seleccionado = seleccionadas.Contains(r.Id);
                return dto;
            }).ToList();

            return Ok(resultado);
        }
    }
}
