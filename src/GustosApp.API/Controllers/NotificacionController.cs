using GustosApp.API.DTO;
using GustosApp.Application.Tests.mocks;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : Controller
    {
        private readonly CrearNotificacionUseCase _crearNotificacion;
        private readonly ObtenerNotificacionUsuarioUseCase _ObtenerNotificaciones;
        private readonly MarcarNotificacionLeidaUseCase _marcarNotificion;

        public NotificacionController(CrearNotificacionUseCase crearNotificacion, ObtenerNotificacionUsuarioUseCase obtenerNotificaciones, MarcarNotificacionLeidaUseCase marcarNotificion)
        {
            _crearNotificacion = crearNotificacion;
            _ObtenerNotificaciones = obtenerNotificaciones;
            _marcarNotificion = marcarNotificion;
        }

        [HttpPost("solicitud")]
        public async Task<IActionResult> CrearNotificacion([FromBody] CrearNotificacionRequest request)
        {
            await _crearNotificacion.HandleAsync(
         request.UsuarioDestinoId,
         request.TipoNotificacion,
         request.nombreUsuario ?? string.Empty,
         request.nombreGrupo ?? string.Empty,
         CancellationToken.None
         );
            return Ok("Notificacion creada");
        }
        /*
        [HttpGet("{usuarioId:guid}")]
        public async Task<IActionResult> ObtenerNotificaciones(Guid usuarioId, CancellationToken ct)
        {
            var notificaciones = await _ObtenerNotificaciones.HandleAsync(usuarioId, ct);

            if (notificaciones == null || !notificaciones.Any())
                return NotFound("No hay notificaciones para este usuario.");
            
            return Ok(notificaciones);
        }

        [HttpPatch("{usuarioId:guid}/marcar-todas-leidas")]
        public async Task<IActionResult> MarcarTodasLeidas(Guid usuarioId,CancellationToken ct)
        {
            await _marcarNotificion.HandleAsync(usuarioId, ct);
            return NoContent();
        }
    }*/
    }
}
