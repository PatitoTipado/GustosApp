using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GustosApp.Application.DTO;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
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
        private readonly IMapper _mapper;


        public AmistadController(EnviarSolicitudAmistadUseCase enviarSolicitud,
                ObtenerSolicitudesPendientesUseCase obtenerPendientes,
                AceptarSolicitudUseCase aceptarSolicitud,
                RechazarSolicitudUseCase rechazarSolicitud,
                ObtenerAmigosUseCase obtenerAmigos,
                EliminarAmigoUseCase eliminarAmigo,
                IUsuarioRepository usuarioRepository,
                IMapper mapper)
        {
            _enviarSolicitud = enviarSolicitud;
            _obtenerPendientes = obtenerPendientes;
            _aceptarSolicitud = aceptarSolicitud;
            _rechazarSolicitud = rechazarSolicitud;
            _obtenerAmigos = obtenerAmigos;
            _eliminarAmigo = eliminarAmigo;
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

       
        [Authorize]
        [HttpGet("buscar-usuarios")]
        public async Task<IActionResult> BuscarUsuarios([FromQuery] string? username, CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();

            if (!string.IsNullOrWhiteSpace(username) && username.Length < 2)
                return BadRequest(new { message = "El nombre de usuario debe tener al menos 2 caracteres para la búsqueda." });
          
            // Obtener usuario actual (para excluirlo de resultados)
            var usuarioActual = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuarioActual == null)
                return Unauthorized("Usuario no encontrado o token inválido.");

            IEnumerable<Usuario> usuarios;

            if (string.IsNullOrWhiteSpace(username))
            {
                // Si no se pasa 'username', devolver los primeros 50 usuarios distintos al actual
                usuarios = await _usuarioRepository.GetAllExceptAsync(usuarioActual.Id, 50, ct);
            }
            else
            {
                // Buscar coincidencias parciales por username (username)
                usuarios = await _usuarioRepository.BuscarPorUsernameAsync(username, usuarioActual.Id, ct);
            }

            var results = usuarios.Select(u => new UsuarioSimpleResponse
            {
                Id = u.Id,
                Nombre = $"{u.Nombre} {u.Apellido}",
                Email = u.Email,
                FotoPerfilUrl = u.FotoPerfilUrl,
                Username = u.IdUsuario
            });

            return Ok(results);
        }

        [Authorize]
        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarSolicitud([FromBody] EnviarSolicitudRequest request, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var resp = await _enviarSolicitud.HandleAsync(uid, request, ct);
            return Ok(resp);
        }
        [Authorize]
        [HttpGet("solicitudes")]
        public async Task<IActionResult> ObtenerSolicitudes(CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var resp = await _obtenerPendientes.HandleAsync(uid, ct);
            return Ok(resp);
        }
        [Authorize]
        [HttpPost("{solicitudId}/aceptar")]
        public async Task<IActionResult> Aceptar(Guid solicitudId, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var resp = await _aceptarSolicitud.HandleAsync(uid, solicitudId, ct);
            return Ok(resp);
        }
        [Authorize]
        [HttpPost("{solicitudId}/rechazar")]
        public async Task<IActionResult> Rechazar(Guid solicitudId, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var resp = await _rechazarSolicitud.HandleAsync(uid, solicitudId, ct);
            return Ok(resp);
        }

        [Authorize]
        [HttpGet("amigos")]
        public async Task<IActionResult> ObtenerAmigos(CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var listaAmigos = await _obtenerAmigos.HandleAsync(uid, ct);

            //automapper
            var dtoListaAmigos= _mapper.Map<List<UsuarioSimpleResponse>>(listaAmigos);

            return Ok(dtoListaAmigos);
        }

        [Authorize]
        [HttpDelete("{username}")]
        public async Task<IActionResult> EliminarAmigo(string username, CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();

            // Buscar usuario actual autenticado
            var usuarioActual = await _usuarioRepository.GetByFirebaseUidAsync(firebaseUid, ct);
            if (usuarioActual == null)
                return Unauthorized("Usuario no encontrado o token inválido.");

            // Buscar amigo por username (IdUsuario)
            var amigo = await _usuarioRepository.GetByUsernameAsync(username, ct);
            if (amigo == null)
                return NotFound(new { message = $"No se encontró el usuario con username '{username}'." });

            // Evitar autoeliminación 
            if (amigo.Id == usuarioActual.Id)
                return BadRequest(new { message = "No podés eliminarte a vos mismo como amigo." });

            var ok = await _eliminarAmigo.HandleAsync(firebaseUid, amigo.Id, ct);

            return Ok(new { success = ok, message = $"Eliminaste a {username} de tu lista de amigos." });
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