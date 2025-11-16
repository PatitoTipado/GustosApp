using System.Security.Claims;
using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RestriccionController : BaseApiController
    {
        private readonly GuardarRestriccionesUseCase _saveRestr;
        private readonly ObtenerRestriccionesUseCase _obtenerRestricciones;
        private readonly ObtenerUsuarioUseCase _usuario; 
        private readonly IMapper _mapper;

        public RestriccionController(GuardarRestriccionesUseCase saveRestr,
            ObtenerRestriccionesUseCase obtenerRestricciones
            ,ObtenerUsuarioUseCase usuario, IMapper mapper)
        {
            _saveRestr = saveRestr;
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
            
            var uid = GetFirebaseUid();

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });


            var usuario = await _usuario.HandleAsync(FirebaseUid: uid, ct:ct)
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

        [Authorize]
        [HttpPut("restricciones")]
        [ProducesResponseType(typeof(GuardarRestriccionesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GuardarRestricciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var restriccionesGuardadas = await _saveRestr.HandleAsync(uid, req.Ids, req.Skip, ModoPreferencias.Edicion, ct);

            var response = new GuardarRestriccionesResponse
            {
                Mensaje = "Restricciones guardadas correctamente",
                GustosRemovidos = restriccionesGuardadas
            };
            return Ok(response);
        }

    }
}
