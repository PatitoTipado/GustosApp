
using Azure.Core;
using System.Security.Claims;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using GustosApp.Infraestructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GustosApp.Infraestructure.Repositories;

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly RegistrarUsuarioUseCase _registrar;
        private readonly UsuarioRepositoryEF _repoUser;


        public UsuarioController(RegistrarUsuarioUseCase context, UsuarioRepositoryEF repoUser)
        {
            _repoUser= repoUser;
            _registrar = context;
        }

        [HttpGet]
        public string Hola()
        {
            return "Hola soy GustoApp";
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
        [Authorize]
        [HttpGet("miperfil")]
        public async Task<IActionResult> MiPerfil(CancellationToken ct)
        {
            var firebaseUid = User.FindFirst("user_id")?.Value;

            var usuario = await _repoUser.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuario == null) return NotFound();

            return Ok(new UsuarioResponse(usuario.Id, usuario.FirebaseUid, usuario.Email, usuario.Nombre,usuario.Apellido,usuario.IdUsuario, usuario.FotoPerfilUrl));
        }
    }
}