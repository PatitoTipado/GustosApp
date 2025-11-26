
using GustosApp.API.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.UsuarioUseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutenticacionController : BaseApiController
    {
        private readonly IFirebaseAuthService _firebase;
        private readonly ObtenerUsuarioUseCase _obtenerUsuario;

        public AutenticacionController(IFirebaseAuthService firebase, ObtenerUsuarioUseCase obtenerUsuario)
        {
            _obtenerUsuario = obtenerUsuario;
            _firebase = firebase;


        }
       
        [Authorize]
        [HttpPost("refresh-claims")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshClaims(CancellationToken ct)
        {
            var uid = GetFirebaseUid(); if (uid == null) return Unauthorized("UID inválido");

            var usuario = await _obtenerUsuario.HandleAsync(FirebaseUid :uid, ct:ct); 
            if (usuario == null) return Unauthorized("Usuario no encontrado"); 
          await _firebase.SetUserRoleAsync(usuario.FirebaseUid, usuario.Rol.ToString()); 

            return Ok(new { message = "Claims actualizados", rol = usuario.Rol.ToString() });
        }
    }
}
