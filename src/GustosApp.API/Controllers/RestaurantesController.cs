using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;
using GustosApp.Domain.Model.@enum;
using GustosApp.Domain.Common;
using GustosApp.Application.UseCases.RestauranteUseCases.SolicitudRestauranteUseCases;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using GustosApp.API.DTO;
using System.Globalization;


// Controlador para restaurantes que se registran en la app por un usuario y restaurantes traidos de Places v1

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantesController : BaseApiController
    {
        private readonly IServicioRestaurantes _servicio;
        private readonly ObtenerUsuarioUseCase _obtenerUsuario;
        private readonly SugerirGustosSobreUnRadioUseCase _sugerirGustos;
        private readonly BuscarRestaurantesCercanosUseCase _buscarRestaurantes;
        private readonly ActualizarDetallesRestauranteUseCase _obtenerDetalles;
        private readonly ConstruirPreferenciasUseCase _construirPreferencias;
        private readonly CrearSolicitudRestauranteUseCase _solicitudesRestaurantes;
        private readonly BuscarRestaurantesUseCase _buscarRestaurante;
        private readonly IAlmacenamientoArchivos _fileStorage;
        private readonly IFileStorageService _firebase;
        private readonly GustosApp.Infraestructure.GustosDbContext _db;
        private readonly ObtenerDatosRegistroRestauranteUseCase _getDatosRegistroRestaurante;
        private readonly IOcrService _ocr;
        private readonly IMenuParser _menuParser;
        private readonly ICacheService _cache;

        private IMapper _mapper;
        private readonly AgregarUsuarioRestauranteFavoritoUseCase _agregarFavoritoUseCase;

        private readonly RegistrarTop3IndividualRestaurantesUseCase _registrarTop3IndividualUseCase;
        private readonly RegistrarVisitaPerfilRestauranteUseCase _registrarVisitaPerfilUseCase;
        private readonly ObtenerMetricasRestauranteUseCase _obtenerMetricasRestauranteUseCase;



        public RestaurantesController(
     IServicioRestaurantes servicio,
      ObtenerUsuarioUseCase obtenerUsuario,
     SugerirGustosSobreUnRadioUseCase sugerirGustos,
     BuscarRestaurantesCercanosUseCase buscarRestaurantes,
     ConstruirPreferenciasUseCase construirPreferencias,
     ActualizarDetallesRestauranteUseCase obtenerDetalles,
     IAlmacenamientoArchivos fileStorage,
      IFileStorageService firebase,
      CrearSolicitudRestauranteUseCase solicitudesRestaurantes,
      ObtenerDatosRegistroRestauranteUseCase getDatosRegistroRestaurante,
        GustosApp.Infraestructure.GustosDbContext db,
        ICacheService cache,
     IOcrService ocr,
     IMenuParser menuParser,
     IMapper mapper, BuscarRestaurantesUseCase buscarRestaurante, AgregarUsuarioRestauranteFavoritoUseCase agregarUsuarioRestauranteFavoritoUseCase,
      RegistrarTop3IndividualRestaurantesUseCase registrarTop3IndividualUseCase,
    RegistrarVisitaPerfilRestauranteUseCase registrarVisitaPerfilUseCase,
    ObtenerMetricasRestauranteUseCase obtenerMetricasRestauranteUseCase)
        {
            _servicio = servicio;
            _obtenerUsuario = obtenerUsuario;
            _sugerirGustos = sugerirGustos;
            _buscarRestaurantes = buscarRestaurantes;
            _construirPreferencias = construirPreferencias;
            _solicitudesRestaurantes = solicitudesRestaurantes;
            _getDatosRegistroRestaurante = getDatosRegistroRestaurante;
            _obtenerDetalles = obtenerDetalles;
            _fileStorage = fileStorage;
            _firebase = firebase;
            _db = db;
            _fileStorage = fileStorage;
            _cache = cache;
            _mapper = mapper;
            _ocr = ocr;
            _menuParser = menuParser;
            _buscarRestaurante = buscarRestaurante;
            _agregarFavoritoUseCase = agregarUsuarioRestauranteFavoritoUseCase;
            _registrarTop3IndividualUseCase = registrarTop3IndividualUseCase;
            _registrarVisitaPerfilUseCase = registrarVisitaPerfilUseCase;
            _obtenerMetricasRestauranteUseCase = obtenerMetricasRestauranteUseCase;
        }
        [Authorize]

        private async Task<(bool ok, string? uid)> CheckOwnerAsync(Guid restauranteId, System.Security.Claims.ClaimsPrincipal user, System.Threading.CancellationToken ct)
        {
            var uid = user.FindFirst("user_id")?.Value
                      ?? user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(uid)) return (false, null);
            var r = await _db.Restaurantes.FirstOrDefaultAsync(x => x.Id == restauranteId, ct);
            if (r == null) return (false, uid);
            var esAdmin = user.IsInRole("admin") || user.Claims.Any(c => c.Type == "role" && c.Value == "admin");
            if (r.PropietarioUid != uid && !esAdmin) return (false, uid);
            return (true, uid);
        }
        [HttpGet("cercanos")]
        public async Task<IActionResult> GetCercanos(double lat, double lng, int radio = 2000, string? types = null, string? priceLevels = null,
                    bool? openNow = null, double? minRating = null, int minUserRatings = 0, string? serves = null,
                    CancellationToken ct = default)
        {

            var result = await _buscarRestaurantes.HandleAsync(lat, lng, radio, types, priceLevels, openNow, minRating, minUserRatings, serves, ct);

            var response = _mapper.Map<List<RestauranteListadoDto>>(result);
            return Ok(new { count = response.Count, restaurantes = response });


        }



        [HttpGet("detalles")]
        public async Task<IActionResult> GetDetalles([FromQuery] string placeId, CancellationToken ct = default)
        {
            var restaurante = await _obtenerDetalles.HandleAsync(placeId, ct);

            var detalles = _mapper.Map<RestauranteListadoDto>(restaurante);
            return Ok(new { message = "Detalles actualizados", detalles });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(
      [FromQuery] List<string>? gustos,
       [FromQuery] string? amigoUsername,
      CancellationToken ct,
     [FromQuery] string? tipoDeRestaurante,
      [FromQuery] double rating,
      [FromQuery(Name = "near.lat")] double? lat,
      [FromQuery(Name = "near.lng")] double? lng,
      [FromQuery(Name = "radiusMeters")] int? radius = 3000,
      [FromQuery] int top = 10
  )
        {

            var firebaseUid = GetFirebaseUid();

            // Filtrar restaurantes cercanos
            var res = await _servicio.BuscarAsync(
                rating: rating,
                tipo: tipoDeRestaurante,
                plato: "",
                lat: lat,
                lng: lng,
                radioMetros: radius
              );



            await _cache.SetAsync(
             $"usuario:{firebaseUid}:location",
             new UserLocation
             (
                lat ?? 0,
                lng ?? 0,
                radius ?? 3000,
                DateTime.UtcNow
             ),
              TimeSpan.FromMinutes(10));

            var preferencias = await _construirPreferencias.HandleAsync(
                firebaseUid,
                amigoUsername: amigoUsername,
                grupoId: null,
                gustosDelFiltro: gustos,
                ct);

            //  Algoritmo combinado
            var recommendations = await _sugerirGustos.Handle(
                preferencias,
                res,
                top,
                ct
            );

            // DTO
            var response = _mapper.Map<List<RestauranteDTO>>(recommendations);

            //registrar cuantos restaurantes salieron en el top 3 individual
            var top3Ids = response
                .Take(3)
                .Select(r => r.Id)
                .ToList();

            if (top3Ids.Count > 0)
            {
                await _registrarTop3IndividualUseCase.HandleAsync(top3Ids, ct);
            }

            return Ok(new
            {
                total = response.Count,
                recomendaciones = response
            });

        }


        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var uid = GetFirebaseUid();

         
            var restaurante = await _servicio.ObtenerAsync(id);

            if (restaurante == null)
                return NotFound("Restaurante no encontrado");

            // 2) Si es GOOGLE y NO tiene reviews locales → Fetch Google Places
            if (!string.IsNullOrWhiteSpace(restaurante.PlaceId) &&
                (restaurante.Reviews == null || !restaurante.Reviews.Any()))
            {
                var actualizado = await _servicio.ObtenerResenasDesdeGooglePlaces(restaurante.PlaceId, ct);
                if (actualizado != null)
                    restaurante = actualizado; 
            }

          
            restaurante.Reviews = restaurante.Reviews
                .OrderBy(r => !r.EsImportada)             
                .ThenByDescending(r => r.FechaCreacion)  
                .ToList();

            //registrar visita al perfil
            if (!string.IsNullOrEmpty(uid))
            {
                await _registrarVisitaPerfilUseCase.HandleAsync(id, ct);
            }

            var dto = _mapper.Map<RestauranteDetalleDto>(restaurante);

            
            return Ok(dto);
        }


     

        [HttpPost]
        [Authorize]
        [RequestSizeLimit(50 * 1024 * 1024)]
        public async Task<IActionResult> CrearSolicitud(
         [FromForm] CrearRestauranteDto dto,
             CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var usuario = await _obtenerUsuario.HandleAsync(FirebaseUid: uid, ct: ct);
            // Parsear las coordenadas manualmente con InvariantCulture
            double? lat = null;
            double? lng = null;

            if (!string.IsNullOrWhiteSpace(dto.Lat))
            {
                // Reemplazar coma por punto y parsear con InvariantCulture
                var latStr = dto.Lat.Replace(",", ".");
                lat = double.Parse(latStr, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrWhiteSpace(dto.Lng))
            {
                var lngStr = dto.Lng.Replace(",", ".");
                lng = double.Parse(lngStr, CultureInfo.InvariantCulture);
            }
            if (usuario.Rol != RolUsuario.Usuario)
                return BadRequest("Ya hiciste una solicitud o sos dueño de un restaurante.");

            var imagenes = new List<SolicitudRestauranteImagen>();

            var urlsSubidas = new List<string>();

            try
            {
                if (dto.ImagenDestacada != null)
                    imagenes.Add(await SubirImagenAsync(dto.ImagenDestacada, TipoImagenSolicitud.Destacada, urlsSubidas));

                if (dto.ImagenesInterior != null)
                    foreach (var file in dto.ImagenesInterior)
                        imagenes.Add(await SubirImagenAsync(file, TipoImagenSolicitud.Interior, urlsSubidas));

                if (dto.ImagenesComidas != null)
                    foreach (var file in dto.ImagenesComidas)
                        imagenes.Add(await SubirImagenAsync(file, TipoImagenSolicitud.Comida, urlsSubidas));

                if (dto.ImagenMenu != null)
                    imagenes.Add(await SubirImagenAsync(dto.ImagenMenu, TipoImagenSolicitud.Menu, urlsSubidas));

                if (dto.Logo != null)
                    imagenes.Add(await SubirImagenAsync(dto.Logo, TipoImagenSolicitud.Logo, urlsSubidas));


                var response = await _solicitudesRestaurantes.HandleAsync(uid, dto.Nombre, dto.Direccion,
                    lat, lng, dto.HorariosJson, dto.GustosQueSirveIds,
                    dto.RestriccionesQueRespetaIds, imagenes,dto.WebsiteUrl ,ct);

                return Ok(response);
            }
            catch (Exception ex)
            {
                foreach (var url in urlsSubidas)
                {
                    try { await _firebase.DeleteFileAsync(url); }
                    catch { }
                }

                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("registro-datos")]
        public async Task<IActionResult> ObtenerDatosParaRegistro(CancellationToken ct)
        {
            var guid = GetFirebaseUid();

            var (gustos, restricciones) = await _getDatosRegistroRestaurante.HandleAsync(ct);

            var dto = new DatosSolicitudRestauranteDto
            {
                Gustos = gustos.Select(g => new ItemSimpleDto
                {
                    Id = g.Id,
                    Nombre = g.Nombre
                }).ToList(),
                Restricciones = restricciones.Select(r => new ItemSimpleDto
                {
                    Id = r.Id,
                    Nombre = r.Nombre
                }).ToList()
            };

            return Ok(dto);
        }

        

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var uid = User.FindFirst("user_id")?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();

            var esAdmin = User.IsInRole("admin") || User.Claims.Any(c => c.Type == "role" && c.Value == "admin");

            var ok = await _servicio.EliminarAsync(id, uid, esAdmin);
            return ok ? NoContent() : NotFound();
        }


        private async Task<SolicitudRestauranteImagen> SubirImagenAsync(IFormFile archivo,
            TipoImagenSolicitud tipo, List<string> urlsSubidas)

        {
            using var stream = archivo.OpenReadStream();
            var url = await _firebase.UploadFileAsync(stream, archivo.FileName, "solicitudes");
            urlsSubidas.Add(url);
            return new SolicitudRestauranteImagen
            {
                Tipo = tipo,
                Url = url
            };
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string texto, CancellationToken ct)
        {
            var restaurantes = await _buscarRestaurante.HandleAsync(texto, ct);

            var dto = restaurantes.Select(r => new RestauranteResponse
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Categoria = r.Categoria,
                Rating = r.Rating,
                Direccion = r.Direccion,
                ImagenUrl = r.ImagenUrl
            }).ToList();

            return Ok(dto);
        }

        [HttpPost("{restauranteId}/favorito")]
        public async Task<IActionResult> AgregarFavorito(Guid restauranteId)
        {
            var firebaseUid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _agregarFavoritoUseCase.HandleAsync(firebaseUid, restauranteId);
            return Ok();
        }

        [HttpGet("{id:guid}/metricas")]
        public async Task<ActionResult> ObtenerMetricas(
            Guid id,
            CancellationToken ct)
        {
            var metricas = await _obtenerMetricasRestauranteUseCase.HandleAsync(id, ct);

            var rest = new RestauranteMetricasDashboardResponse
            {
                RestauranteId = metricas.RestauranteId,
                TotalTop3Individual = metricas.Estadisticas?.TotalTop3Individual ?? 0,
                TotalTop3Grupo = metricas.Estadisticas?.TotalTop3Grupo ?? 0,
                TotalVisitasPerfil = metricas.Estadisticas?.TotalVisitasPerfil ?? 0,
                TotalFavoritosHistorico = metricas.TotalFavoritos,
                TotalFavoritosActual = metricas.TotalFavoritos
            };

            return Ok(rest);
        }

    }


}

