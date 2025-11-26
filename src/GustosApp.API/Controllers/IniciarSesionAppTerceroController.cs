using Azure.Core;
using GustosApp.API.DTO;
using GustosApp.Application.UseCases.UsuarioUseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [Authorize]
    public class IniciarSesionAppTerceroController : Controller
    {

        private readonly AppLoginConFirebaseUseCase _appLoginConFirebaseUseCase;

        public IniciarSesionAppTerceroController(AppLoginConFirebaseUseCase appLoginConFirebaseUseCase)
        {
            _appLoginConFirebaseUseCase = appLoginConFirebaseUseCase;
        }

        [HttpPost("firebase-login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginConFirebase([FromBody] LoginFirebaseRequest request)
        {
            try
            {
                var firebaseUid = await _appLoginConFirebaseUseCase.HandleAsync(request.IdToken);

                if (request.Tipo == "Usuario")
                    return Redirect("/tercero/usuario");
                else if (request.Tipo == "Restaurante")
                    return Redirect("/tercero/restaurante");

                return BadRequest("Tipo de registro inválido");

            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido");
            }
        }

        [HttpPost("tercero/usuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CrearUsuario(string IdToken)
        {
            var usuario = await _appLoginConFirebaseUseCase.HandleAsyncCrearUsuario(IdToken);
            return Ok(usuario);
        }

        [HttpPost("tercero/restaurante")]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CrearRestaurante(string IdToken)
        {
            var restaurante = await _appLoginConFirebaseUseCase.HandleAsyncCrearRestaurante(IdToken);
            return Ok(restaurante);
        }
    }
}
