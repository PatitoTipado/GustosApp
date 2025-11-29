
using Azure.Core;
using System.Security.Claims;
using GustosApp.API.DTO;
using GustosApp.Infraestructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GustosApp.Infraestructure.Repositories;
using GustosApp.Domain.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Http.HttpResults;
using GustosApp.Domain.Model;
using GustosApp.API.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using GustosApp.Application.UseCases.AmistadUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases;

using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.RestriccionesUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.CondicionesMedicasUseCases;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuarioController : BaseApiController
    {

        private readonly GuardarRestriccionesUseCase _saveRestr;
        private readonly GuardarCondicionesUseCase _saveCond;
        private readonly ObtenerGustosFiltradosUseCase _getGustos;
        private readonly GuardarGustosUseCase _saveGustos;
        private readonly ObtenerResumenRegistroUseCase _resumen;
        private readonly FinalizarRegistroUseCase _finalizar;
        private readonly RegistrarUsuarioUseCase _registrar;
        private readonly ObtenerUsuarioUseCase _obtenerUsuario;
        private readonly ConfirmarAmistadEntreUsuarios _confirmarAmistadEntreUsuarios;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;



        public UsuarioController(RegistrarUsuarioUseCase context, GuardarRestriccionesUseCase saveRestr, GuardarCondicionesUseCase saveCond,
            ObtenerGustosFiltradosUseCase getGustos, GuardarGustosUseCase saveGustos, ObtenerResumenRegistroUseCase resumen, FinalizarRegistroUseCase finalizar,
            IMapper mapper, IUsuarioRepository usuarioRepository,
            ObtenerUsuarioUseCase obtenerUsuario, ConfirmarAmistadEntreUsuarios confirmarAmistadEntreUsuarios,
            ICacheService cache)
        {

            _registrar = context;
            _saveRestr = saveRestr;
            _saveCond = saveCond;
            _getGustos = getGustos;
            _finalizar = finalizar;
            _saveGustos = saveGustos;
            _resumen = resumen;
            _mapper = mapper;
            _obtenerUsuario = obtenerUsuario;
            _confirmarAmistadEntreUsuarios = confirmarAmistadEntreUsuarios;
            _cache = cache;
        }

        [Authorize]
        [HttpPost("registrar")]
        //Documentacion swagger statuscodes
        [ProducesResponseType(typeof(RegistrarUsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var firebaseUid = GetFirebaseUid();

            //automapper para mapear request a usuario para el caso de uso
            var user = _mapper.Map<Usuario>(request);

            var usuarioGuardado = await _registrar.HandleAsync(firebaseUid, user, ct);

            var response = new RegistrarUsuarioResponse
            {
                Message = "Usuario registrado exitosamente.",
                //mapeo el usuario que devuelve el usecase a un DTO para el front
                Usuario = _mapper.Map<UsuarioResponse>(usuarioGuardado)

            };
            return Ok(response);
        }


        [Authorize(Policy = "RegistroIncompleto")]
        [HttpPost("restricciones")]
        [ProducesResponseType(typeof(GuardarRestriccionesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GuardarRestricciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var restriccionesGuardadas = await _saveRestr.HandleAsync(uid, req.Ids, req.Skip, ModoPreferencias.Registro, ct);

            var response = new GuardarRestriccionesResponse
            {
                Mensaje = "Restricciones guardadas correctamente",
                GustosRemovidos = restriccionesGuardadas
            };
            return Ok(response);
        }

        [Authorize(Policy = "RegistroIncompleto")]
        [HttpPost("condiciones")]
        [ProducesResponseType(typeof(GuardarCondicionesResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GuardarCondiciones([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            var condiciones = await _saveCond.HandleAsync(uid, req.Ids, req.Skip, ModoPreferencias.Registro, ct);

            var response = new GuardarCondicionesResponse
            {
                Mensaje = "Condiciones médicas guardadas correctamente",
                GustosRemovidos = condiciones
            };


            return Ok(response);

        }




        [Authorize(Policy = "RegistroIncompleto")]
        [HttpPost("gustos")]
        [ProducesResponseType(typeof(GuardarGustosResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GuardarGustos([FromBody] GuardarIdsRequest req, CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var gustos = await _saveGustos.HandleAsync(uid, req.Ids, ModoPreferencias.Registro, ct);

            var response = new GuardarGustosResponse
            {
                Mensaje = "Gustos guardados correctamente",
                GustosIncompatibles = gustos
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet("resumen")]
        [ProducesResponseType(typeof(UsuarioResumenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Resumen(
      [FromQuery] string modo = "registro",
      CancellationToken ct = default)
        {
            var uid = GetFirebaseUid();

            var usuario = await _resumen.HandleAsync(uid, modo, ct);

            var response = _mapper.Map<UsuarioResumenResponse>(usuario);
            return Ok(response);
        }


        [Authorize(Policy = "RegistroIncompleto")]
        [HttpPost("finalizar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Finalizar(CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            await _finalizar.HandleAsync(uid, ct);
            return Ok(new { mensaje = "Registro finalizado", pasoActual = "Finalizado" });
        }

        [HttpGet("{username}/perfil")]
        [Authorize]
        [ProducesResponseType(typeof(UsuarioPerfilResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPerfilUsuario(
      [FromRoute] string username,
      CancellationToken ct = default)
        {
            var currentUid = GetFirebaseUid();
            if (string.IsNullOrWhiteSpace(currentUid))
                return Unauthorized(new { mensaje = "UID inválido" });

      
            var usuarioActual = await _obtenerUsuario.HandleAsync(
                FirebaseUid: currentUid,
                ct: ct
            );

            if (usuarioActual == null)
                return Unauthorized(new { mensaje = "Usuario autenticado no encontrado" });

         
            var usuarioPerfil = await _obtenerUsuario.HandleAsync(
                Username: username,
                ct: ct
            );

            if (usuarioPerfil == null)
                return NotFound(new { mensaje = "Usuario no encontrado" });

            var esMiPerfil = usuarioPerfil.FirebaseUid == usuarioActual.FirebaseUid;

            var amistad = await _confirmarAmistadEntreUsuarios.HandleAsync(
                usuarioActual.Id,
                usuarioPerfil.Id,
                ct
            );

            var esAmigo = amistad != null && amistad.Estado == EstadoSolicitud.Aceptada;

            bool puedeVerCompleto = esMiPerfil || esAmigo || !usuarioPerfil.EsPrivado;


            if (!puedeVerCompleto)
            {
                return Ok(new UsuarioPerfilResponse
                {
                    Nombre = usuarioPerfil.Nombre,
                    Apellido = usuarioPerfil.Apellido,
                    Username = usuarioPerfil.IdUsuario,
                    FotoPerfilUrl = usuarioPerfil.FotoPerfilUrl,
                    EsPrivado = usuarioPerfil.EsPrivado,
                    EsMiPerfil = esMiPerfil,
                    EsAmigo = esAmigo,
                    Gustos = new(),
                    Visitados = new()
                });
            }

       
            usuarioPerfil = await _obtenerUsuario.HandleWithVisitadosAsync(
                Username: username,
                ct: ct
            );

            var resp = _mapper.Map<UsuarioPerfilResponse>(usuarioPerfil);

            resp.EsMiPerfil = esMiPerfil;
            resp.EsAmigo = esAmigo;

            return Ok(resp);
        }



        [Authorize]
        [HttpGet("estado-registro")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EstadoRegistro(CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            //  Intentar leer estado desde Redis
            var cacheKey = $"registro:{uid}:inicialCompleto";
            var cacheValue = await _cache.GetAsync<bool?>(cacheKey);

            bool registroCompleto;

            if (cacheValue != null)
            {
                registroCompleto = cacheValue.Value;
            }
            else
            {
                try
                {
                    //  Si no existe en redis → consulto DB
                    var usuario = await _obtenerUsuario.HandleAsync(FirebaseUid: uid, ct: ct);

                    registroCompleto = usuario.Gustos.Count >= 3;

                    //  Guardar en redis para próximas requests
                    await _cache.SetAsync(
                        cacheKey,
                        registroCompleto,
                        TimeSpan.FromHours(12)
                    );
                }
                catch (Exception ex) when (ex.Message == "Usuario no encontrado")
                {
                    // Si el usuario no existe en la BD, su registro claramente no está completo
                    Console.WriteLine($"[EstadoRegistro] Usuario {uid} no existe en BD, retornando registroCompleto=false");
                    registroCompleto = false;
                }
            }

            return Ok(new
            {
                registroCompleto
            });
        }


        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
        {
            var uid = GetFirebaseUid();
            if (string.IsNullOrWhiteSpace(uid))
                return Unauthorized(new { error = "UID inválido" });

            var usuario = await _obtenerUsuario.HandleAsync(FirebaseUid: uid, ct: ct);
            if (usuario == null)
                return Unauthorized(new { error = "Usuario no encontrado" });

            var response = new
            {
                id = usuario.Id,
                idUsuario = usuario.IdUsuario,
                nombre = usuario.Nombre,
                apellido = usuario.Apellido,
                fotoPerfilUrl = usuario.FotoPerfilUrl,
                esPremium = usuario.EsPremium() 
            };

            return Ok(response);
        }


    }
}


    















