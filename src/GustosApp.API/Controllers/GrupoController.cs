using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.Handlers;
using GustosApp.Application.Services;
using GustosApp.Application.UseCases;
using GustosApp.Application.UseCases.GrupoUseCases;
using GustosApp.Application.UseCases.GrupoUseCases.ChatGrupoUseCases;
using GustosApp.Application.UseCases.GrupoUseCases.InvitacionGrupoUseCases;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;
using GustosApp.Application.UseCases.UsuarioUseCases;

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class GrupoController : BaseApiController
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
        private readonly ObtenerRestaurantesAleatoriosGrupoUseCase _obtenerRestaurantesAleatorios;
        private readonly ConstruirPreferenciasUseCase _construirPreferencias;
        private readonly ActualizarNombreGrupoUseCase _actualizarNombreGrupoUseCase;
        private readonly RegistrarTop3GrupoRestaurantesUseCase _registrarTop3GrupoRestaurantesUseCase;


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
            IMapper mapper,
            ObtenerRestaurantesAleatoriosGrupoUseCase obtenerRestaurantesAleatorios,
           ConstruirPreferenciasUseCase construirPreferencias,
            ActualizarNombreGrupoUseCase actualizarNombreGrupoUseCase,
            RegistrarTop3GrupoRestaurantesUseCase registrarTop3GrupoRestaurantesUseCase


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
            _obtenerRestaurantesAleatorios = obtenerRestaurantesAleatorios;
            _construirPreferencias = construirPreferencias;
            _servicioPreferenciasGrupos = servicioPreferenciasGrupos;
            _actualizarNombreGrupoUseCase = actualizarNombreGrupoUseCase;
            _registrarTop3GrupoRestaurantesUseCase = registrarTop3GrupoRestaurantesUseCase;

        }
        [Authorize]
        [HttpPost("crear")]
        [ProducesResponseType(typeof(GrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LimiteGruposAlcanzadoResponse), StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CrearGrupo([FromBody] CrearGrupoRequest request, CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();

            var grupo = await _crearGrupoUseCase.HandleAsync(firebaseUid, request.Nombre, request.Descripcion, ct);

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

             request.UsuarioId, request.UsuarioUsername, request.MensajePersonalizado, ct);

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
        [HttpPut("{grupoId}/nombre")]
        [ProducesResponseType(typeof(GrupoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ActualizarNombreGrupo(Guid grupoId, [FromBody] ActualizarNombreGrupoRequest request, CancellationToken ct)
        {
            var firebaseUid = GetFirebaseUid();
            var grupo = await _actualizarNombreGrupoUseCase.HandleAsync(firebaseUid, grupoId, request.Nombre, ct);
            var response = _mapper.Map<GrupoResponse>(grupo);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

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
            Console.WriteLine($"[GrupoController] ObtenerGrupo - FirebaseUid: {firebaseUid}, GrupoId: {grupoId}");

            if (!Guid.TryParse(grupoId, out var gid))
            {
                Console.WriteLine($"[GrupoController] Invalid GUID: {grupoId}");
                return BadRequest("El id de grupo no es un GUID válido");
            }


            bool esMiembro = await _verificacionMiembroGrupo.HandleAsync(firebaseUid, gid, ct);
            Console.WriteLine($"[GrupoController] Usuario {firebaseUid} es miembro del grupo {gid}: {esMiembro}");

            if (!esMiembro)
            {
                Console.WriteLine($"[GrupoController] Returning 403 - User not member of group");
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "No eres miembro de este grupo." });
            }

            var grupo = await _obtenerGrupoDetalleUseCase.HandleAsync(firebaseUid, gid, ct);
            Console.WriteLine($"[GrupoController] Group details retrieved successfully");

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
        public async Task<IActionResult> agregarGustoDeGrupo(Guid grupoId, List<string> gustos)
        {
            var firebaseUid = GetFirebaseUid();
            var ok = await _servicioPreferenciasGrupos.ActualizarGustosDeGrupo(gustos, grupoId, firebaseUid);

            var response = new ActualizarGustosGrupoResponse
            {
                Success = ok,
                Mensaje = "Gustos actualizados correctamente"
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPut("desactivarMiembro")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DesactivarMiembro(Guid grupoId, Guid UsuarioId)
        {
            var firebaseUid = GetFirebaseUid();
            var ok = await _servicioPreferenciasGrupos.DesactivarMiembroDeGrupo(grupoId, UsuarioId, firebaseUid);

            var response = new
            {
                Success = ok,
                Mensaje = "Miembro Desactivado Correctamente"
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPut("activarMiembro")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ActivarMiembro(Guid grupoId, Guid UsuarioId)
        {
            var firebaseUid = GetFirebaseUid();
            var ok = await _servicioPreferenciasGrupos.ActivarMiembro(grupoId, UsuarioId, firebaseUid);

            var response = new
            {
                Success = ok,
                Mensaje = "Miembro activado Correctamente"
            };

            return Ok(response);
        }


        [Authorize]
        [HttpGet("restaurantes/{grupoId}")]
        [ProducesResponseType(typeof(IEnumerable<RestauranteDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RecomendarRestauranteGrupo(Guid grupoId,
            [FromQuery(Name = "near.lat")] double? lat = -34.641812775271,
            [FromQuery(Name = "near.lng")] double? lng = -58.56990230458638,
            [FromQuery(Name = "radiusMeters")] int? radius = 1000,
            [FromQuery] int top = 10,
             CancellationToken ct = default)
        {
            var firebaseUid = GetFirebaseUid();

            // Buscar restaurantes desde la infraestructura
            var restaurantes = await _servicio.BuscarAsync(0, "", "", lat, lng, radius);


            // Obtener preferencias del grupo
            var preferencias = await _construirPreferencias.HandleAsync(
            firebaseUid,
            null,
             grupoId,
             null,
             ct);


            // Obtener recomendaciones
            var recomendados = await _sugerirGustos.Handle(preferencias, restaurantes, top, ct);

            // Mapear al DTO
            var response = _mapper.Map<List<RestauranteDTO>>(recomendados);

            // registrar Top 3 grupal
            var top3Ids = response
                .Take(3)
                .Select(r => r.Id)    
                .ToList();

            if (top3Ids.Count > 0)
            {
                await _registrarTop3GrupoRestaurantesUseCase.HandleAsync(top3Ids, ct);
            }

            return Ok(response);
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
