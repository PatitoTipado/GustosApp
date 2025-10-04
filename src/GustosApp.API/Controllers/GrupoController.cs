using System.Security.Claims;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class GrupoController : ControllerBase
    {
        private readonly CrearGrupoUseCase _crearGrupoUseCase;
        private readonly InvitarUsuarioGrupoUseCase _invitarUsuarioUseCase;
        private readonly UnirseGrupoUseCase _unirseGrupoUseCase;
        private readonly AbandonarGrupoUseCase _abandonarGrupoUseCase;
        private readonly ObtenerGruposUsuarioUseCase _obtenerGruposUseCase;
        private readonly ObtenerInvitacionesUsuarioUseCase _obtenerInvitacionesUseCase;
        private readonly AceptarInvitacionGrupoUseCase _aceptarInvitacionUseCase;

        public GrupoController(
            CrearGrupoUseCase crearGrupoUseCase,
            InvitarUsuarioGrupoUseCase invitarUsuarioUseCase,
            UnirseGrupoUseCase unirseGrupoUseCase,
            AbandonarGrupoUseCase abandonarGrupoUseCase,
            ObtenerGruposUsuarioUseCase obtenerGruposUseCase,
            ObtenerInvitacionesUsuarioUseCase obtenerInvitacionesUseCase,
            AceptarInvitacionGrupoUseCase aceptarInvitacionUseCase)
        {
            _crearGrupoUseCase = crearGrupoUseCase;
            _invitarUsuarioUseCase = invitarUsuarioUseCase;
            _unirseGrupoUseCase = unirseGrupoUseCase;
            _abandonarGrupoUseCase = abandonarGrupoUseCase;
            _obtenerGruposUseCase = obtenerGruposUseCase;
            _obtenerInvitacionesUseCase = obtenerInvitacionesUseCase;
            _aceptarInvitacionUseCase = aceptarInvitacionUseCase;
        }

        [HttpPost("crear-prueba")]
        [AllowAnonymous]
        public async Task<IActionResult> CrearGrupoPrueba([FromBody] CrearGrupoRequest request, CancellationToken ct)
        {
            try
            {
                // Para pruebas, usar un Firebase UID fijo
                var firebaseUid = "test-user-123";
                var resultado = await _crearGrupoUseCase.HandleAsync(firebaseUid, request, ct);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("test-simple")]
        [AllowAnonymous]
        public IActionResult TestSimple([FromBody] CrearGrupoRequest request)
        {
            return Ok(new { 
                message = "Endpoint funcionando correctamente", 
                data = request,
                timestamp = DateTime.UtcNow
            });
        }

        [HttpPost("simular-grupo")]
        [AllowAnonymous]
        public IActionResult SimularGrupo([FromBody] CrearGrupoRequest request)
        {
            var grupoSimulado = new
            {
                id = Guid.NewGuid(),
                nombre = request.Nombre,
                descripcion = request.Descripcion,
                administradorId = Guid.NewGuid(),
                administradorNombre = "Usuario de Prueba",
                fechaCreacion = DateTime.UtcNow,
                activo = true,
                codigoInvitacion = "ABC12345",
                fechaExpiracionCodigo = DateTime.UtcNow.AddDays(7),
                cantidadMiembros = 1,
                miembros = new[]
                {
                    new
                    {
                        id = Guid.NewGuid(),
                        usuarioId = Guid.NewGuid(),
                        usuarioNombre = "Usuario de Prueba",
                        usuarioEmail = "test@ejemplo.com",
                        fechaUnion = DateTime.UtcNow,
                        esAdministrador = true
                    }
                }
            };

            return Ok(grupoSimulado);
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearGrupo([FromBody] CrearGrupoRequest request, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                var resultado = await _crearGrupoUseCase.HandleAsync(firebaseUid, request, ct);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("{grupoId}/invitar")]
        public async Task<IActionResult> InvitarUsuario(Guid grupoId, [FromBody] InvitacionGrupoRequest request, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                var resultado = await _invitarUsuarioUseCase.HandleAsync(firebaseUid, grupoId, request, ct);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("unirse")]
        public async Task<IActionResult> UnirseGrupo([FromBody] UnirseGrupoRequest request, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                var resultado = await _unirseGrupoUseCase.HandleAsync(firebaseUid, request, ct);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("{grupoId}/abandonar")]
        public async Task<IActionResult> AbandonarGrupo(Guid grupoId, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                var resultado = await _abandonarGrupoUseCase.HandleAsync(firebaseUid, grupoId, ct);
                return Ok(new { success = resultado, message = "Has abandonado el grupo exitosamente" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("mis-grupos")]
        public async Task<IActionResult> ObtenerMisGrupos(CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                var resultado = await _obtenerGruposUseCase.HandleAsync(firebaseUid, ct);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("invitaciones")]
        public async Task<IActionResult> ObtenerInvitaciones(CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                var resultado = await _obtenerInvitacionesUseCase.HandleAsync(firebaseUid, ct);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost("invitaciones/{invitacionId}/aceptar")]
        public async Task<IActionResult> AceptarInvitacion(Guid invitacionId, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                var resultado = await _aceptarInvitacionUseCase.HandleAsync(firebaseUid, invitacionId, ct);
                return Ok(resultado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        private string GetFirebaseUid()
        {
            var firebaseUid = User.FindFirst("user_id")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                throw new UnauthorizedAccessException("No se encontró el UID de Firebase en el token.");

            return firebaseUid;
        }
    }
}
