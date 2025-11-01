
using Azure.Core;
using System.Security.Claims;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using GustosApp.Infraestructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GustosApp.Infraestructure.Repositories;
using GustosApp.Domain.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http.HttpResults;
using GustosApp.Domain.Model;
using GustosApp.API.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly GuardarRestriccionesUseCase _saveRestr;
        private readonly GuardarCondicionesUseCase _saveCond;
        private readonly ObtenerGustosFiltradosUseCase _getGustos;
        private readonly GuardarGustosUseCase _saveGustos;
        private readonly ObtenerResumenRegistroUseCase _resumen;
        private readonly FinalizarRegistroUseCase _finalizar;
        private readonly RegistrarUsuarioUseCase _registrar;
        private readonly IMapper _mapper;

        private readonly IUsuarioRepository _usuarioRepository;



        public UsuarioController(RegistrarUsuarioUseCase context, GuardarRestriccionesUseCase saveRestr, GuardarCondicionesUseCase saveCond,
            ObtenerGustosFiltradosUseCase getGustos, GuardarGustosUseCase saveGustos, ObtenerResumenRegistroUseCase resumen, FinalizarRegistroUseCase finalizar,
            IMapper mapper, IUsuarioRepository usuarioRepository)
        {
    
            _registrar = context;
            _saveRestr = saveRestr;
            _saveCond = saveCond;
            _getGustos = getGustos;
            _finalizar = finalizar;
            _saveGustos = saveGustos;
            _resumen = resumen;
            _mapper = mapper;
             _usuarioRepository = usuarioRepository;
        }

        [Authorize]
        [HttpPost("registrar")]

        //Documentacion swagger statuscodes
        [ProducesResponseType(typeof(RegistrarUsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // Firebase suele mapear el UID en el claim "user_id" (también puede venir en "sub")
            var firebaseUid = User.FindFirst("user_id")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                return Unauthorized(new { message = "No se encontró el UID de Firebase en el token." });

            //automapper para mapear request a usuario para el caso de uso
            var user = _mapper.Map<Usuario>(request);

            var usuarioGuardado = await _registrar.HandleAsync(firebaseUid, user, ct);

            var response = new RegistrarUsuarioResponse
            {
                Message = "Usuario registrado exitosamente.",
                //mapeo el usuario que devuelve el usecase a un DTO para el front
                Usuario = _mapper.Map<UsuarioResponse>(usuarioGuardado)
               
            };
            return Ok(response);
        }


        [Authorize]
        [HttpPost("restricciones")]
        [ProducesResponseType(typeof(GuardarRestriccionesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GuardarRestricciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var restriccionesGuardadas= await _saveRestr.HandleAsync(uid, req.Ids,req.Skip, ct);

            var response = new GuardarRestriccionesResponse
            {
                Mensaje = "Restricciones guardadas correctamente",
                GustosRemovidos = restriccionesGuardadas
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPost("condiciones")]
        [ProducesResponseType(typeof(GuardarCondicionesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GuardarCondiciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var condiciones= await _saveCond.HandleAsync(uid, req.Ids, req.Skip, ct);

            var response = new GuardarCondicionesResponse
            {
                Mensaje = "Condiciones médicas guardadas correctamente",
                GustosRemovidos = condiciones
            };

            
            return Ok(response);
            
        }

       


        [Authorize]
        [HttpPost("gustos")]
        [ProducesResponseType(typeof(GuardarGustosResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GuardarGustos([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var gustos=await _saveGustos.HandleAsync(uid, req.Ids, ct);
            
            var response= new GuardarGustosResponse
            {
                Mensaje = "Gustos guardados correctamente",
                GustosIncompatibles = gustos
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("resumen")]
        [ProducesResponseType(typeof(UsuarioResumenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Resumen(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });


            var resumenUsuario = await _resumen.HandleAsync(uid, ct);

             var response = _mapper.Map<UsuarioResumenResponse>(resumenUsuario);
            return Ok(response);
       
        }

        [Authorize]
        [HttpPost("finalizar")]
        public async Task<IActionResult> Finalizar(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                     ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });


            await _finalizar.HandleAsync(uid, ct);
            return Ok(new { mensaje = "Registro finalizado", pasoActual = "Finalizado" });
        }

         [HttpGet("{username}/perfil")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPerfilUsuario([FromRoute] string username, CancellationToken ct = default)
        {
            var user = await _usuarioRepository.GetByUsernameAsync(username, ct);
            if (user is null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            var resp = new UsuarioPerfilResponse
            {
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Username = user.IdUsuario,
                FotoPerfilUrl = user.FotoPerfilUrl,
                EsPrivado = user.EsPrivado,
                Gustos = (user.Gustos ?? new System.Collections.Generic.List<Gusto>())
                    .Select(g => new GustoLiteDto { Id = g.Id, Nombre = g.Nombre })
                    .ToList(),
                Visitados = (user.Visitados ?? new System.Collections.Generic.List<UsuarioRestauranteVisitado>())
                    .Select(v => new VisitadoDto
                    {
                        Id = !string.IsNullOrWhiteSpace(v.PlaceId)
                            ? v.PlaceId!
                            : (v.RestauranteId.HasValue ? v.RestauranteId.Value.ToString() : v.Id.ToString()),
                        Nombre = v.Nombre,
                        Lat = v.Latitud,
                        Lng = v.Longitud
                    })
                    .ToList()
            };

            return Ok(resp);
        }
}

}

