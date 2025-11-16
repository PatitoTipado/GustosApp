using System.Security.Claims;
using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CondicionMedicaController : BaseApiController
    {

        private readonly GuardarCondicionesUseCase _saveCond;
        private readonly ObtenerCondicionesMedicasUseCase _obtenerCondicionesMed;
        private readonly IMapper _mapper;

        public CondicionMedicaController(
            GuardarCondicionesUseCase saveCond,
            ObtenerCondicionesMedicasUseCase obtenerCondicionesMed,
            IMapper mapper)
        {
            _saveCond = saveCond;
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

        [Authorize]
        [HttpPut("condiciones")]
        [ProducesResponseType(typeof(GuardarCondicionesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GuardarCondiciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var condiciones = await _saveCond.HandleAsync(uid, req.Ids, req.Skip, ModoPreferencias.Edicion, ct);

            var response = new GuardarCondicionesResponse
            {
                Mensaje = "Condiciones médicas guardadas correctamente",
                GustosRemovidos = condiciones
            };


            return Ok(response);

        }
        
    }
}
