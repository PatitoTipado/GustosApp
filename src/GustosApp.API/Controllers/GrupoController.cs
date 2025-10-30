using GustosApp.Application.DTO;
using GustosApp.Application.Services;
using GustosApp.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        private ActualizarGustosAGrupoUseCase _actualizarGustosGrupoUseCase;
        private readonly IServicioRestaurantes _servicio;
        private readonly ObtenerPreferenciasGruposUseCase _obtenerPreferenciasGrupos;
        private readonly SugerirGustosSobreUnRadioUseCase _sugerirGustos;
        private readonly ObtenerRestaurantesAleatoriosGrupoUseCase _obtenerRestaurantesAleatorios;

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
            ActualizarGustosAGrupoUseCase actualizarGustosAGrupoUseCase,
            IServicioRestaurantes servicio,
            SugerirGustosSobreUnRadioUseCase sugerirGustos,
            ObtenerPreferenciasGruposUseCase obtenerGustos,
            ObtenerRestaurantesAleatoriosGrupoUseCase obtenerRestaurantesAleatorios
            )
        {
            _servicio = servicio;
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
            _actualizarGustosGrupoUseCase = actualizarGustosAGrupoUseCase;
            _obtenerPreferenciasGrupos = obtenerGustos;
            _sugerirGustos = sugerirGustos;
            _obtenerRestaurantesAleatorios = obtenerRestaurantesAleatorios;

        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearGrupo([FromBody] CrearGrupoRequest request, CancellationToken ct)
        {
            
                var firebaseUid = GetFirebaseUid();
                var resultado = await _crearGrupoUseCase.HandleAsync(firebaseUid, request, ct);
                return Ok(resultado);
           
        }

        [HttpPost("{grupoId}/invitar")]
        public async Task<IActionResult> InvitarUsuario(Guid grupoId, [FromBody] InvitacionGrupoRequest request, CancellationToken ct)
        {
           
                if ((request.EmailUsuario == null || string.IsNullOrWhiteSpace(request.EmailUsuario)) && (!request.UsuarioId.HasValue || request.UsuarioId == Guid.Empty) && (request.UsuarioUsername == null || string.IsNullOrWhiteSpace(request.UsuarioUsername)))
                {
                    return BadRequest("Se debe proporcionar UsuarioId, UsuarioUsername o EmailUsuario para invitar");
                }
                var firebaseUid = GetFirebaseUid();
                var resultado = await _invitarUsuarioUseCase.HandleAsync(firebaseUid, grupoId, request, ct);
                return Ok(resultado);
           
        }
        [Authorize]
        [HttpPost("unirse")]
        public async Task<IActionResult> UnirseGrupo([FromBody] UnirseGrupoRequest request, CancellationToken ct)
        {
           
                var firebaseUid = GetFirebaseUid();
                var resultado = await _unirseGrupoUseCase.HandleAsync(firebaseUid, request, ct);
                return Ok(resultado);
         
        }

        [HttpPost("{grupoId}/abandonar")]
        public async Task<IActionResult> AbandonarGrupo(Guid grupoId, CancellationToken ct)
        {
           
                var firebaseUid = GetFirebaseUid();
                var resultado = await _abandonarGrupoUseCase.HandleAsync(firebaseUid, grupoId, ct);
                return Ok(new { success = resultado, message = "Has abandonado el grupo exitosamente" });
         
        }
        [Authorize]
        [HttpGet("mis-grupos")]
        public async Task<IActionResult> ObtenerMisGrupos(CancellationToken ct)
        {
                var firebaseUid = GetFirebaseUid();

                var resultado = await _obtenerGruposUseCase.HandleAsync(firebaseUid, ct);

                return Ok(resultado);
          
        }

        [HttpGet("invitaciones")]
        public async Task<IActionResult> ObtenerInvitaciones(CancellationToken ct)
        {
           
                var firebaseUid = GetFirebaseUid();
                var resultado = await _obtenerInvitacionesUseCase.HandleAsync(firebaseUid, ct);
                return Ok(resultado);
           
        }

        [HttpDelete("{grupoId}")]
        public async Task<IActionResult> EliminarGrupo(string grupoId, CancellationToken ct)
        {
           
                var firebaseUid = GetFirebaseUid();

                if (!Guid.TryParse(grupoId, out var gid))
                return BadRequest("El id de grupo no es un GUID válido");

                var ok = await _eliminarGrupoUseCase.HandleAsync(firebaseUid, gid, ct);
                return Ok(new { success = ok });
          
        }

        [HttpDelete("{grupoId}/miembros/{usuarioId}")]
        public async Task<IActionResult> RemoverMiembro(string grupoId, string usuarioId, CancellationToken ct)
        {
           
                var firebaseUid = GetFirebaseUid();
                if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");
                if (!Guid.TryParse(usuarioId, out var uid)) return BadRequest("El id de usuario no es un GUID válido");
                var ok = await _removerMiembroUseCase.HandleAsync(firebaseUid, gid, uid, ct);
                return Ok(new { success = ok });
          
        }

        [HttpGet("{grupoId}/chat")]
        public async Task<IActionResult> ObtenerChat(string grupoId, CancellationToken ct)
        {
           
                var firebaseUid = GetFirebaseUid();
                if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");
                var msgs = await _obtenerChatGrupoUseCase.HandleAsync(firebaseUid, gid, ct);
                return Ok(msgs);
          
        }

        [HttpGet("{grupoId}")]
        public async Task<IActionResult> ObtenerGrupo(string grupoId, CancellationToken ct)
        {
          
                var firebaseUid = GetFirebaseUid();
                if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");
                var detalle = await _obtenerGrupoDetalleUseCase.HandleAsync(firebaseUid, gid, ct);
                return Ok(detalle);
  
        }

     

        [HttpPost("invitaciones/{invitacionId}/aceptar")]
        public async Task<IActionResult> AceptarInvitacion(Guid invitacionId, CancellationToken ct)
        {
          
                var firebaseUid = GetFirebaseUid();
                var resultado = await _aceptarInvitacionUseCase.HandleAsync(firebaseUid, invitacionId, ct);
                return Ok(resultado);
           
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

        [HttpPut("actualizarGustos")]
        public async Task<IActionResult> agregarGustoDeGrupo(Guid grupoId,List<string> gustos)
        {
            var firebaseUid = GetFirebaseUid();
            var resultado = await _actualizarGustosGrupoUseCase.Handle(gustos,grupoId);
            return Ok(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> recomendarRestauranteGrupo(CancellationToken ct,
        [FromBody]Guid grupoId,
        [FromQuery(Name = "near.lat")] double? lat = -34.641812775271,
        [FromQuery(Name = "near.lng")] double? lng = -58.56990230458638,
        [FromQuery(Name = "radiusMeters")] int? radius = 1000,
        [FromQuery] string? tipo = "",
        [FromQuery] string? plato = "",
        [FromQuery] int top = 10
 )
        {
            var res = await _servicio.BuscarAsync(
                tipo: tipo,
                plato: plato,
                lat: lat,
                lng: lng,
                radioMetros: radius
            );

            Console.WriteLine(res.ToList().Count());
            Console.WriteLine(res.ToList().Count());
            Console.WriteLine(res.ToList().Count());

            var firebaseUid = User.FindFirst("user_id")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
            {
                return Unauthorized(new { message = "No se encontró el UID de Firebase en el token." });
            }

            var preferenciasDTO = await _obtenerPreferenciasGrupos.Handle(grupoId,ct);

            Console.WriteLine(preferenciasDTO.ToString());
            Console.WriteLine(preferenciasDTO.ToString());
            Console.WriteLine(preferenciasDTO.ToString());

            var recommendations = _sugerirGustos.Handle(preferenciasDTO, res.ToList(), top, ct);

            Console.WriteLine(recommendations.ToList().Count());
            Console.WriteLine(recommendations.ToList().Count());
            Console.WriteLine(recommendations.ToList().Count());

            foreach (var item in recommendations)
            {
                foreach (var gusto in item.GustosQueSirve)
                {
                    Console.WriteLine(gusto.Nombre);
                }
            }

            return Ok(recommendations);
        }

        /// <summary>
        /// Obtiene restaurantes aleatorios basados en las preferencias del grupo
        /// </summary>
        /// <param name="grupoId">ID del grupo</param>
        /// <param name="request">Parámetros de búsqueda (cantidad, ubicación opcional)</param>
        /// <param name="ct">Token de cancelación</param>
        /// <returns>Lista de restaurantes aleatorios que coinciden con los gustos del grupo</returns>
        /// <response code="200">Retorna la lista de restaurantes encontrados</response>
        /// <response code="400">Si el grupoId es inválido o el grupo no existe</response>
        /// <response code="401">Si el token de autenticación es inválido</response>
        /// <response code="403">Si el usuario no es miembro del grupo</response>
        /// <response code="500">Si ocurre un error interno del servidor</response>
        [HttpPost("{grupoId}/restaurantes-aleatorios")]
        [ProducesResponseType(typeof(List<RestauranteAleatorioResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ObtenerRestaurantesAleatorios(
            Guid grupoId, 
            [FromBody] ObtenerRestaurantesAleatoriosRequest request, 
            CancellationToken ct)
        {
            try
            {
                var firebaseUid = GetFirebaseUid();
                
                // Verificar que el usuario es miembro del grupo
                var usuario = await _obtenerGruposUseCase.HandleAsync(firebaseUid, ct);
                var esMiembro = usuario.Any(g => g.Id == grupoId);
                
                if (!esMiembro)
                {
                    return Forbid("No eres miembro de este grupo");
                }

                var restaurantes = await _obtenerRestaurantesAleatorios.HandleAsync(grupoId, request, ct);
                return Ok(restaurantes);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener restaurantes: {ex.Message}");
            }
        }
    }
}
