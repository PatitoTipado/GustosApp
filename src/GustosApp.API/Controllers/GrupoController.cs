using System.Security.Claims;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
        private readonly EliminarGrupoUseCase _eliminarGrupoUseCase;
        private readonly ObtenerChatGrupoUseCase _obtenerChatGrupoUseCase;
        private readonly EnviarMensajeGrupoUseCase _enviarMensajeGrupoUseCase;
        private readonly ObtenerGrupoDetalleUseCase _obtenerGrupoDetalleUseCase;
        private readonly RemoverMiembroGrupoUseCase _removerMiembroUseCase;
        private AgregarGustosAGrupoUseCase _agregarGustosAGrupoUseCase;

        public GrupoController(
            CrearGrupoUseCase crearGrupoUseCase,
            InvitarUsuarioGrupoUseCase invitarUsuarioUseCase,
            UnirseGrupoUseCase unirseGrupoUseCase,
            AbandonarGrupoUseCase abandonarGrupoUseCase,
            ObtenerGruposUsuarioUseCase obtenerGruposUseCase,
            ObtenerInvitacionesUsuarioUseCase obtenerInvitacionesUseCase,
            AceptarInvitacionGrupoUseCase aceptarInvitacionUseCase,
            EliminarGrupoUseCase eliminarGrupoUseCase,
            ObtenerGrupoDetalleUseCase obtenerGrupoDetalleUseCase,
            RemoverMiembroGrupoUseCase removerMiembroUseCase,
            ObtenerChatGrupoUseCase obtenerChatGrupoUseCase,
            EnviarMensajeGrupoUseCase enviarMensajeGrupoUseCase,
            AgregarGustosAGrupoUseCase agregarGustosAGrupoUseCase
            )
        {
            _crearGrupoUseCase = crearGrupoUseCase;
            _invitarUsuarioUseCase = invitarUsuarioUseCase;
            _unirseGrupoUseCase = unirseGrupoUseCase;
            _abandonarGrupoUseCase = abandonarGrupoUseCase;
            _obtenerGruposUseCase = obtenerGruposUseCase;
            _obtenerInvitacionesUseCase = obtenerInvitacionesUseCase;
            _aceptarInvitacionUseCase = aceptarInvitacionUseCase;
            _eliminarGrupoUseCase = eliminarGrupoUseCase;
            _obtenerGrupoDetalleUseCase = obtenerGrupoDetalleUseCase;
            _removerMiembroUseCase = removerMiembroUseCase;
            _obtenerChatGrupoUseCase = obtenerChatGrupoUseCase;
            _enviarMensajeGrupoUseCase = enviarMensajeGrupoUseCase;
            _agregarGustosAGrupoUseCase = agregarGustosAGrupoUseCase;
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
                // Validate request contains either UsuarioId or EmailUsuario
                if ((request.EmailUsuario == null || string.IsNullOrWhiteSpace(request.EmailUsuario)) && (!request.UsuarioId.HasValue || request.UsuarioId == Guid.Empty) && (request.UsuarioUsername == null || string.IsNullOrWhiteSpace(request.UsuarioUsername)))
                {
                    return BadRequest("Se debe proporcionar UsuarioId, UsuarioUsername o EmailUsuario para invitar");
                }
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

        [HttpDelete("{grupoId}")]
        public async Task<IActionResult> EliminarGrupo(string grupoId, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");
                var ok = await _eliminarGrupoUseCase.HandleAsync(firebaseUid, gid, ct);
                return Ok(new { success = ok });
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, "Error interno del servidor"); }
        }

        [HttpDelete("{grupoId}/miembros/{usuarioId}")]
        public async Task<IActionResult> RemoverMiembro(string grupoId, string usuarioId, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");
                if (!Guid.TryParse(usuarioId, out var uid)) return BadRequest("El id de usuario no es un GUID válido");
                var ok = await _removerMiembroUseCase.HandleAsync(firebaseUid, gid, uid, ct);
                return Ok(new { success = ok });
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, "Error interno del servidor"); }
        }

        [HttpGet("{grupoId}/chat")]
        public async Task<IActionResult> ObtenerChat(string grupoId, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");
                var msgs = await _obtenerChatGrupoUseCase.HandleAsync(firebaseUid, gid, ct);
                return Ok(msgs);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) {
                // Dev: return full exception to help debugging local issues (e.g., missing DB table)
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpGet("{grupoId}")]
        public async Task<IActionResult> ObtenerGrupo(string grupoId, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");
                var detalle = await _obtenerGrupoDetalleUseCase.HandleAsync(firebaseUid, gid, ct);
                return Ok(detalle);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, "Error interno del servidor"); }
        }

        [HttpPost("{grupoId}/chat")]
        public async Task<IActionResult> EnviarMensaje(string grupoId, [FromBody] JsonElement body, CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");
                // Parse body safely whether it's an object like { "mensaje": "..." } or a raw string
                string mensaje = string.Empty;
                try
                {
                    if (body.ValueKind == JsonValueKind.Object)
                    {
                        if (body.TryGetProperty("mensaje", out var p) && p.ValueKind == JsonValueKind.String) mensaje = p.GetString();
                        else if (body.TryGetProperty("Mensaje", out var p2) && p2.ValueKind == JsonValueKind.String) mensaje = p2.GetString();
                        else if (body.TryGetProperty("text", out var p3) && p3.ValueKind == JsonValueKind.String) mensaje = p3.GetString();
                    }
                    else if (body.ValueKind == JsonValueKind.String)
                    {
                        mensaje = body.GetString();
                    }
                }
                catch { /* ignore malformed body */ }

                if (string.IsNullOrWhiteSpace(mensaje)) return BadRequest("Mensaje vacío");
                var saved = await _enviarMensajeGrupoUseCase.HandleAsync(firebaseUid, gid, mensaje, ct);
                return Ok(saved);
            }
            catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (Exception ex) { return StatusCode(500, ex.ToString()); }
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

        [HttpPost("grupo/agregarGusto")]
        public async Task<IActionResult> agregarGustoDeGrupo(Guid grupoId,[FromBody] UsuarioPreferenciasDTO preferencias)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                var invitado = await _agregarGustosAGrupoUseCase.Handle(preferencias,grupoId);
                return Ok(invitado);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
