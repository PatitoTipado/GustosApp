using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Application.Handlers;
using GustosApp.Application.Services;
using GustosApp.Application.UseCases;
using GustosApp.Domain.Model;
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
        private readonly ObtenerGrupoDetalleUseCase _obtenerGrupoDetalleUseCase;
        private readonly RemoverMiembroGrupoUseCase _removerMiembroUseCase;
        private readonly IServicioRestaurantes _servicio;
        private readonly ObtenerPreferenciasGruposUseCase _obtenerPreferenciasGrupos;
        private readonly SugerirGustosSobreUnRadioUseCase _sugerirGustos;
        private readonly VerificarSiMiembroEstaEnGrupoUseCase _verificacionMiembroGrupo;
        private readonly IServicioPreferenciasGrupos _servicioPreferenciasGrupos;
        private readonly EnviarMensajeGrupoUseCase _enviarMensajeGrupoUseCase;
        private readonly IMapper _mapper;

       

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
            IServicioRestaurantes servicio,
            SugerirGustosSobreUnRadioUseCase sugerirGustos,
            ObtenerPreferenciasGruposUseCase obtenerGustos,
            IServicioPreferenciasGrupos servicioPreferenciasGrupos,
            VerificarSiMiembroEstaEnGrupoUseCase verificacionMiembroGrupo,
             ActualizarGustosAGrupoUseCase actualizarGustosAGrupoUseCase,
            IMapper mapper
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
            _obtenerPreferenciasGrupos = obtenerGustos;
            _sugerirGustos = sugerirGustos;
            _verificacionMiembroGrupo = verificacionMiembroGrupo;
            _mapper = mapper;

            _servicioPreferenciasGrupos = servicioPreferenciasGrupos;
        }
        [Authorize]
        [HttpPost("crear")]
        [ProducesResponseType(typeof(GrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LimiteGruposAlcanzadoResponse), StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CrearGrupo([FromBody] CrearGrupoRequest request, CancellationToken ct)
        {
                var firebaseUid = GetFirebaseUid();

                var grupo = await _crearGrupoUseCase.HandleAsync(firebaseUid, request.Nombre,request.Descripcion, ct);

            var response = _mapper.Map<GrupoResponse>(grupo);
           
            return Ok(response);
         
        }
        [Authorize]
        [HttpPost("{grupoId}/invitar")]
        [ProducesResponseType(typeof(InvitacionGrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> InvitarUsuario(Guid grupoId, [FromBody] InvitacionGrupoRequest request, CancellationToken ct)
        {
           
                if ((request.EmailUsuario == null || string.IsNullOrWhiteSpace(request.EmailUsuario)) && (!request.UsuarioId.HasValue 
                || request.UsuarioId == Guid.Empty) && (request.UsuarioUsername == null || string.IsNullOrWhiteSpace(request.UsuarioUsername)))
                {
                    return BadRequest("Se debe proporcionar UsuarioId, UsuarioUsername o EmailUsuario para invitar");
                }
                var firebaseUid = GetFirebaseUid();
                var invitacion = await _invitarUsuarioUseCase.HandleAsync(firebaseUid, grupoId, request.EmailUsuario,
                  
                 request.UsuarioId,request.UsuarioUsername,request.MensajePersonalizado, ct);
               
                 var response = _mapper.Map<InvitacionGrupoResponse>(invitacion);

            return Ok(response);
           
        }


        [Authorize]
        [HttpPost("unirse")]
        [ProducesResponseType(typeof(GrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LimiteGruposAlcanzadoResponse), StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UnirseGrupo([FromBody] UnirseGrupoRequest request, CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();
            var grupo = await _unirseGrupoUseCase.HandleAsync(firebaseUid, request, ct);

            var response = _mapper.Map<GrupoResponse>(grupo);

            return Ok(response);
        }
        [Authorize]
        [HttpPost("{grupoId}/abandonar")]
        [ProducesResponseType(typeof(AbandonarGrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AbandonarGrupo(Guid grupoId, CancellationToken ct)
        {
           
                var firebaseUid = GetFirebaseUid();
                var resultado = await _abandonarGrupoUseCase.HandleAsync(firebaseUid, grupoId, ct);

            var response = new AbandonarGrupoResponse
            {
                Success = resultado,
                Mensaje = "Has abandonado el grupo exitosamente"
            };
                return Ok(response);
         
        }
        [Authorize]
        [HttpGet("mis-grupos")]
        [ProducesResponseType(typeof(List<GrupoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerMisGrupos(CancellationToken ct)
        {
                var firebaseUid = GetFirebaseUid();

                var grupos = await _obtenerGruposUseCase.HandleAsync(firebaseUid, ct);

                 var response = _mapper.Map<List<GrupoResponse>>(grupos);

            return Ok(response);
          
        }
        [Authorize]
        [HttpGet("invitaciones")]
        [ProducesResponseType(typeof(IEnumerable<InvitacionGrupoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerInvitaciones(CancellationToken ct)
        {
           
                var firebaseUid = GetFirebaseUid();
                var invitaciones = await _obtenerInvitacionesUseCase.HandleAsync(firebaseUid, ct);

            var response = _mapper.Map<IEnumerable<InvitacionGrupoResponse>>(invitaciones);
            return Ok(response);
           
        }
        [Authorize]
        [HttpDelete("{grupoId}")]
        [ProducesResponseType(typeof(EliminarGrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EliminarGrupo(string grupoId, CancellationToken ct)
        {
           
                var firebaseUid = GetFirebaseUid();

                if (!Guid.TryParse(grupoId, out var gid))
                return BadRequest("El id de grupo no es un GUID válido");

                var ok = await _eliminarGrupoUseCase.HandleAsync(firebaseUid, gid, ct);

            var response = new EliminarGrupoResponse
            {
                Success = ok,
                Mensaje = "Has eliminado el grupo correctamente"
            };
                return Ok(response);
          
        }

     
        [Authorize]
        [HttpDelete("{grupoId}/miembros/{username}")]
        [ProducesResponseType(typeof(RemoverMiembroResponse), StatusCodes.Status200OK)]

        public async Task<IActionResult> RemoverMiembro(Guid grupoId, string username, CancellationToken ct)
        {
           var firebaseUid = GetFirebaseUid();
               
            //revsisar como hace para sacar a un miembro
            var ok = await _removerMiembroUseCase.HandleAsync(firebaseUid, grupoId, username, ct);
            var response = new RemoverMiembroResponse
            {
                Success = ok,
                Mensaje = "Has removido al miembro  correctamente"
            };

                return Ok(response);
          
        }
        [Authorize]
        [HttpGet("{grupoId}/chat")]
        [ProducesResponseType(typeof(IEnumerable<ChatMensajeResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtenerChat(string grupoId, CancellationToken ct)
        {
           
            var firebaseUid = GetFirebaseUid();
            if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");
               
            var msgs = await _obtenerChatGrupoUseCase.HandleAsync(firebaseUid, gid, ct);

            var response = _mapper.Map<IEnumerable<ChatMensajeResponse>>(msgs);

            return Ok(response);
        }
        [Authorize]
        [HttpGet("{grupoId}")]
        [ProducesResponseType(typeof(GrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> ObtenerGrupo(string grupoId, CancellationToken ct)
        {
                var firebaseUid = GetFirebaseUid();
                if (!Guid.TryParse(grupoId, out var gid)) return BadRequest("El id de grupo no es un GUID válido");

                
              bool esMiembro = await _verificacionMiembroGrupo.HandleAsync(firebaseUid, gid, ct);
            if (!esMiembro)
                return Forbid("No eres miembro de este grupo.");

            var grupo = await _obtenerGrupoDetalleUseCase.HandleAsync(firebaseUid, gid, ct);

            var response = _mapper.Map<GrupoResponse>(grupo);
            return Ok(response);

        }


        [Authorize]
        [HttpPost("invitaciones/{invitacionId}/aceptar")]
        [ProducesResponseType(typeof(GrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AceptarInvitacion(Guid invitacionId, CancellationToken ct)
        {
          
                var firebaseUid = GetFirebaseUid();
                var grupo = await _aceptarInvitacionUseCase.HandleAsync(firebaseUid, invitacionId, ct);
                 var response = _mapper.Map<GrupoResponse>(grupo);
            
            return Ok(response);
            
           
        }
        [Authorize]
        [HttpPut("actualizarGustos")]
        [ProducesResponseType(typeof(ActualizarGustosGrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> agregarGustoDeGrupo(Guid grupoId,List<string> gustos)
        {
            var firebaseUid = GetFirebaseUid();
            var ok = await _servicioPreferenciasGrupos.ActualizarGustosDeGrupo(gustos,grupoId,firebaseUid);

            var response = new ActualizarGustosGrupoResponse
            {
                Success = ok,
                Mensaje = "Gustos actualizados correctamente"
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("restaurantes/{grupoId}")]
        [ProducesResponseType(typeof(IEnumerable<RestauranteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RecomendarRestauranteGrupo(Guid grupoId,
            [FromQuery(Name = "near.lat")] double? lat = -34.641812775271,
            [FromQuery(Name = "near.lng")] double? lng = -58.56990230458638,
            [FromQuery(Name = "radiusMeters")] int? radius = 1000,
            [FromQuery] int top = 10,
             CancellationToken ct = default)
        {
            var firebaseUid = GetFirebaseUid();

            // Buscar restaurantes desde la infraestructura
            var restaurantes = await _servicio.BuscarAsync(0,"", "", lat, lng, radius);

            // Obtener preferencias del grupo
            var preferencias = await _obtenerPreferenciasGrupos.HandleAsync(grupoId, ct);

            // Obtener recomendaciones
            var recomendados = _sugerirGustos.Handle(preferencias, restaurantes, top, ct);

            // Mapear al DTO
            var response = _mapper.Map<List<RestauranteDto>>(recomendados);

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
