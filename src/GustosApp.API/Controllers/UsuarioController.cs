
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




        public UsuarioController(RegistrarUsuarioUseCase context, GuardarRestriccionesUseCase saveRestr, GuardarCondicionesUseCase saveCond,
            ObtenerGustosFiltradosUseCase getGustos, GuardarGustosUseCase saveGustos, ObtenerResumenRegistroUseCase resumen, FinalizarRegistroUseCase finalizar,
            IMapper mapper)
        {
    
            _registrar = context;
            _saveRestr = saveRestr;
            _saveCond = saveCond;
            _getGustos = getGustos;
            _finalizar = finalizar;
            _saveGustos = saveGustos;
            _resumen = resumen;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost("registrar")]
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

            var user = _mapper.Map<Usuario>(request);
            var usuarioGuardado = await _registrar.HandleAsync(firebaseUid, user, ct);

            var response = new RegistrarUsuarioResponse
            {
                Message = "Usuario registrado exitosamente.",
                Usuario = _mapper.Map<UsuarioResponse>(usuarioGuardado)
            };
            return Ok(response);
        }


        [Authorize]
        [HttpPost("restricciones")]
        public async Task<IActionResult> GuardarRestricciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var response= await _saveRestr.HandleAsync(uid, req.Ids,req.Skip, ct);


            var resp = new PasoResponse(
            PasoActual: "Restricciones",
            Next: "/registro/condiciones",
            Data: response.Mensaje,
            Conflictos: response.GustosRemovidos
            );
            return Ok(resp);
        }

        [Authorize]
        [HttpPost("condiciones")]
        public async Task<IActionResult> GuardarCondiciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var response= await _saveCond.HandleAsync(uid, req.Ids, req.Skip, ct);

            var resp = new PasoResponse(
           PasoActual: "Restricciones",
           Next: "/registro/condiciones",
           Data: response.mensaje,
           Conflictos: response.GustosRemovidos
           );
            return Ok(resp);
            
        }

       


        [Authorize]
        [HttpPost("gustos")]
        public async Task<IActionResult> GuardarGustos([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });

            var response=await _saveGustos.HandleAsync(uid, req.Ids, ct);

            var resp = new PasoResponse(
                PasoActual: "Gustos",
                Next: "/registro/resumen",
                Data: "Gustos guardados correctamente",
                Conflictos: response
                );
            return Ok(resp);
        }

        [Authorize]
        [HttpGet("resumen")]
        public async Task<IActionResult> Resumen(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                      ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { message = "Token no válido o sin UID" });


            var r = await _resumen.HandleAsync(uid, ct);
            return Ok(new
            {
                resumen = r
            });
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
    }


}

