using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AmistadController : ControllerBase
    {
        private readonly EnviarSolicitudAmistadUseCase _enviarSolicitud;
        private readonly ObtenerSolicitudesPendientesUseCase _obtenerPendientes;
        private readonly AceptarSolicitudUseCase _aceptarSolicitud;
        private readonly RechazarSolicitudUseCase _rechazarSolicitud;
        private readonly ObtenerAmigosUseCase _obtenerAmigos;
        private readonly EliminarAmigoUseCase _eliminarAmigo;
        private readonly IUsuarioRepository _usuarioRepository;

        public AmistadController(EnviarSolicitudAmistadUseCase enviarSolicitud,
                ObtenerSolicitudesPendientesUseCase obtenerPendientes,
                AceptarSolicitudUseCase aceptarSolicitud,
                RechazarSolicitudUseCase rechazarSolicitud,
                ObtenerAmigosUseCase obtenerAmigos,
                EliminarAmigoUseCase eliminarAmigo,
                IUsuarioRepository usuarioRepository)
        {
            _enviarSolicitud = enviarSolicitud;
            _obtenerPendientes = obtenerPendientes;
            _aceptarSolicitud = aceptarSolicitud;
            _rechazarSolicitud = rechazarSolicitud;
            _obtenerAmigos = obtenerAmigos;
            _eliminarAmigo = eliminarAmigo;
            _usuarioRepository = usuarioRepository;
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

        [HttpGet("buscar-usuarios")]
        public async Task<IActionResult> BuscarUsuarios([FromQuery] string? q, CancellationToken ct)
        {
            GetFirebaseUid();
            var results = new List<UsuarioSimpleResponse>();

            if (string.IsNullOrWhiteSpace(q))
            {
                // devolver todos los usuarios (limitado)
                var todos = await _usuarioRepository.GetAllAsync(100, ct);
                results.AddRange(todos.Select(u => new UsuarioSimpleResponse { Id = u.Id, Nombre = u.Nombre + " " + u.Apellido, Email = u.Email, FotoPerfilUrl = u.FotoPerfilUrl }));
                return Ok(results);
            }

            var usuarioPorEmail = await _usuarioRepository.GetByEmailAsync(q, ct);
            if (usuarioPorEmail != null)
            {
                results.Add(new UsuarioSimpleResponse { Id = usuarioPorEmail.Id, Nombre = usuarioPorEmail.Nombre + " " + usuarioPorEmail.Apellido, Email = usuarioPorEmail.Email, FotoPerfilUrl = usuarioPorEmail.FotoPerfilUrl });
                return Ok(results);
            }

            // Intentar buscar por nombre de usuario (IdUsuario)
            var usuarioPorUsername = await _usuarioRepository.GetByUsernameAsync(q, ct);
            if (usuarioPorUsername != null)
            {
                results.Add(new UsuarioSimpleResponse { Id = usuarioPorUsername.Id, Nombre = usuarioPorUsername.Nombre + " " + usuarioPorUsername.Apellido, Email = usuarioPorUsername.Email, FotoPerfilUrl = usuarioPorUsername.FotoPerfilUrl });
                return Ok(results);
            }

            return Ok(results);
        }

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarSolicitud([FromBody] EnviarSolicitudRequest request, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var resp = await _enviarSolicitud.HandleAsync(uid, request, ct);
            return Ok(resp);
        }

        [HttpGet("solicitudes")]
        public async Task<IActionResult> ObtenerSolicitudes(CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var resp = await _obtenerPendientes.HandleAsync(uid, ct);
            return Ok(resp);
        }

        [HttpPost("{solicitudId}/aceptar")]
        public async Task<IActionResult> Aceptar(Guid solicitudId, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var resp = await _aceptarSolicitud.HandleAsync(uid, solicitudId, ct);
            return Ok(resp);
        }

        [HttpPost("{solicitudId}/rechazar")]
        public async Task<IActionResult> Rechazar(Guid solicitudId, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var resp = await _rechazarSolicitud.HandleAsync(uid, solicitudId, ct);
            return Ok(resp);
        }


        [HttpGet("amigos")]
        public async Task<IActionResult> ObtenerAmigos(CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var resp = await _obtenerAmigos.HandleAsync(uid, ct);
            return Ok(resp);
        }

        [HttpDelete("{amigoId}")]
        public async Task<IActionResult> EliminarAmigo(string amigoId, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            if (!Guid.TryParse(amigoId, out var amigoGuid))
            {
                return BadRequest("El id de amigo no es un GUID válido.");
            }
            var ok = await _eliminarAmigo.HandleAsync(uid, amigoGuid, ct);
            return Ok(new { success = ok });
        }
    }
}