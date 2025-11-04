using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

using GustosApp.API.DTO;
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
        private readonly BuscarUsuariosUseCase _buscarUsuarios;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;


        public AmistadController(EnviarSolicitudAmistadUseCase enviarSolicitud,
                ObtenerSolicitudesPendientesUseCase obtenerPendientes,
                AceptarSolicitudUseCase aceptarSolicitud,
                RechazarSolicitudUseCase rechazarSolicitud,
                ObtenerAmigosUseCase obtenerAmigos,
                EliminarAmigoUseCase eliminarAmigo,
                BuscarUsuariosUseCase buscarUsuarios,
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
            _buscarUsuarios = buscarUsuarios;
            _mapper = mapper;
        }


        /// <summary>
        /// Busca usuarios por nombre de usuario (parcial) o devuelve los primeros 50.
        /// </summary>
        /// <param name="username">Texto parcial del nombre de usuario</param>
        /// <returns>Lista de usuarios encontrados</returns>
        [HttpGet("buscar-usuarios")]
        [ProducesResponseType(typeof(IEnumerable<UsuarioSimpleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BuscarUsuarios([FromQuery] string? username, CancellationToken ct)
        {
            var firebaseUid = User.FindFirst("user_id")?.Value
                            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
                return Unauthorized(new { message = "Token inválido o sin UID." });

                var usuarios = await _buscarUsuarios.HandleAsync(firebaseUid, username, ct);

                var response = _mapper.Map<IEnumerable<UsuarioSimpleResponse>>(usuarios);

                return Ok(response);
          
        }
      
        [Authorize]
        [HttpPost("enviar")]
        [ProducesResponseType(typeof(SolicitudAmistadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EnviarSolicitud([FromBody] EnviarSolicitudRequest request, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var solicitud = await _enviarSolicitud.HandleAsync(uid, request.UsernameDestino,request.Mensaje, ct);
            var response = _mapper.Map<SolicitudAmistadResponse>(solicitud);

            return Ok(response);
        }


        
        [Authorize]
        [HttpGet("solicitudes")]
        [ProducesResponseType(typeof(IEnumerable<SolicitudAmistadResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerSolicitudes(CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var listaSolicitudesAmistad = await _obtenerPendientes.HandleAsync(uid, ct);
            var response = _mapper.Map<IEnumerable<SolicitudAmistadResponse>>(listaSolicitudesAmistad);


            return Ok(response);
        }

        [Authorize]
        [HttpPost("{solicitudId}/aceptar")]
        [ProducesResponseType(typeof(SolicitudAmistadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Aceptar(Guid solicitudId, CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var solicitud = await _aceptarSolicitud.HandleAsync(uid, solicitudId, ct);
            var response = _mapper.Map<SolicitudAmistadResponse>(solicitud);

            return Ok(response);
        }


        [Authorize]
        [HttpPost("{solicitudId}/rechazar")]
        [ProducesResponseType(typeof(SolicitudAmistadResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Rechazar(Guid solicitudId, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var solicitud = await _rechazarSolicitud.HandleAsync(uid, solicitudId, ct);
            var response = _mapper.Map<SolicitudAmistadResponse>(solicitud);


            return Ok(response);
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
        [ProducesResponseType(typeof(EliminarAmigoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            var response = new EliminarAmigoResponse
            {
                Success = ok,
                Message = $"Eliminaste a {username} de tu lista de amigos."
            };

            return Ok(response);
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