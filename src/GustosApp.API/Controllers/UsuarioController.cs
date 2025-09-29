
using Azure.Core;
using System.Security.Claims;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using GustosApp.Infraestructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly RegistrarUsuarioUseCase _registrar;

        public UsuarioController(RegistrarUsuarioUseCase context)
        {
            _registrar = context;
        }

        [Authorize]
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioRequest request,CancellationToken ct)
        {
            // Firebase suele mapear el UID en el claim "user_id" (también puede venir en "sub")
            var firebaseUid = User.FindFirst("user_id")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                return Unauthorized("No se encontró el UID de Firebase en el token.");

            var resp = await _registrar.HandleAsync(firebaseUid, request, ct);
            return Ok(resp);
        }
    }
}