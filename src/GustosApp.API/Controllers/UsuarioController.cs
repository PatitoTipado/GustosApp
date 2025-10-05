
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
     


        public UsuarioController(RegistrarUsuarioUseCase context, GuardarRestriccionesUseCase saveRestr, GuardarCondicionesUseCase saveCond,
            ObtenerGustosFiltradosUseCase getGustos, GuardarGustosUseCase saveGustos, ObtenerResumenRegistroUseCase resumen, FinalizarRegistroUseCase finalizar)
        {
    
            _registrar = context;
            _saveRestr = saveRestr;
            _saveCond = saveCond;
            _getGustos = getGustos;
            _finalizar = finalizar;
            _saveGustos = saveGustos;
            _resumen = resumen;
        }

        [Authorize]
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioRequest request, CancellationToken ct)
        {


            // Firebase suele mapear el UID en el claim "user_id" (también puede venir en "sub")
            var firebaseUid = User.FindFirst("user_id")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                return Unauthorized(new { message = "No se encontró el UID de Firebase en el token." });

            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.Nombre) ||
                string.IsNullOrWhiteSpace(request.Apellido) ||
                string.IsNullOrWhiteSpace(request.Email))
               
            {
                return BadRequest( new { message = "Nombre, Apellido, Email son obligatorios." });
            }

            var resp = await _registrar.HandleAsync(firebaseUid, request, ct);
            return Ok(new
            {
                message = "Usuario registrado correctamente",
                usuario = resp
            });
        }


        [Authorize]
        [HttpPost("restricciones")]
        public async Task<IActionResult> GuardarRestricciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException();
            await _saveRestr.HandleAsync(uid, req.Ids,req.Skip, ct);
            return Ok(new { next = "/registro/condiciones", pasoActual = "Restricciones" });
        }

        [Authorize]
        [HttpPost("condiciones")]
        public async Task<IActionResult> GuardarCondiciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException();
            await _saveCond.HandleAsync(uid, req.Ids, req.Skip, ct);
            return Ok(new { next = "/registro/gustos/filtrados", pasoActual = "Condiciones" });
        }

        /*[Authorize]
        [HttpGet("gustos/filtrados")]
        public async Task<IActionResult> ObtenerGustosFiltrados(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException();
            var resp = await _getGustos.HandleAsync(uid, ct);
            return Ok(resp);
        }
        */

        [Authorize]
        [HttpPost("gustos")]
        public async Task<IActionResult> GuardarGustos([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException();
            await _saveGustos.HandleAsync(uid, req.Ids, ct);
            return Ok(new { next = "/registro/resumen", pasoActual = "Gustos" });
        }

        [Authorize]
        [HttpGet("resumen")]
        public async Task<IActionResult> Resumen(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException();
            var r = await _resumen.HandleAsync(uid, ct);
            return Ok(r);
        }

        [Authorize]
        [HttpPost("finalizar")]
        public async Task<IActionResult> Finalizar(CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value ?? throw new UnauthorizedAccessException();
            await _finalizar.HandleAsync(uid, ct);
            return Ok(new { mensaje = "Registro finalizado", pasoActual = "Finalizado" });
        }
    }


}

