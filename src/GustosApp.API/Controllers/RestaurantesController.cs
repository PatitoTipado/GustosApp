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
using System.Globalization;


// Controlador para restaurantes que se registran en la app por un usuario y restaurantes traidos de Places v1

namespace GustosApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantesController : BaseApiController
    {
        private readonly IServicioRestaurantes _servicio;
        private readonly ObtenerUsuarioUseCase _obtenerUsuario;
        private readonly SugerirGustosSobreUnRadioUseCase _sugerirGustos;
    
        private readonly ConstruirPreferenciasUseCase _construirPreferencias;
        private readonly CrearSolicitudRestauranteUseCase _solicitudesRestaurantes;
        private readonly BuscarRestaurantesUseCase _buscarRestaurante;
        private readonly IFileStorageService _firebaseStorage;

        private readonly IFileStorageService _firebase;
        private readonly GustosApp.Infraestructure.GustosDbContext _db;

        private readonly ObtenerDatosRegistroRestauranteUseCase _getDatosRegistroRestaurante;
        private readonly ICacheService _cache;
        private readonly IMapper _mapper;
        private readonly AgregarUsuarioRestauranteFavoritoUseCase _agregarFavoritoUseCase;
        private readonly RegistrarTop3IndividualRestaurantesUseCase _registrarTop3IndividualUseCase;
        private readonly RegistrarVisitaPerfilRestauranteUseCase _registrarVisitaPerfilUseCase;
        private readonly ObtenerMetricasRestauranteUseCase _obtenerMetricasRestauranteUseCase;
        private readonly ActualizarRestauranteDashboardUseCase _actualizarRestauranteDashboardUseCase;

        private readonly ObtenerRestauranteDetalleUseCase _obtenerRestauranteDetalle;


        public RestaurantesController(
     IServicioRestaurantes servicio,
      ObtenerUsuarioUseCase obtenerUsuario,
     SugerirGustosSobreUnRadioUseCase sugerirGustos,
     ConstruirPreferenciasUseCase construirPreferencias,
    IFileStorageService firebaseStorage,
      CrearSolicitudRestauranteUseCase solicitudesRestaurantes,
      ObtenerDatosRegistroRestauranteUseCase getDatosRegistroRestaurante,
        ICacheService cache,IMapper mapper, BuscarRestaurantesUseCase buscarRestaurante,
       AgregarUsuarioRestauranteFavoritoUseCase agregarUsuarioRestauranteFavoritoUseCase,
      RegistrarTop3IndividualRestaurantesUseCase registrarTop3IndividualUseCase,
    RegistrarVisitaPerfilRestauranteUseCase registrarVisitaPerfilUseCase,
    ObtenerMetricasRestauranteUseCase obtenerMetricasRestauranteUseCase,
    ActualizarRestauranteDashboardUseCase actualizarRestauranteDashboardUseCase,
    ObtenerRestauranteDetalleUseCase obtenerRestauranteDetalle,
    GustosApp.Infraestructure.GustosDbContext db, IFileStorageService firebase)
        {
            _servicio = servicio;
            _obtenerUsuario = obtenerUsuario;
            _sugerirGustos = sugerirGustos;
            _construirPreferencias = construirPreferencias;
            _solicitudesRestaurantes = solicitudesRestaurantes;
            _getDatosRegistroRestaurante = getDatosRegistroRestaurante;
            _firebaseStorage = firebaseStorage;
            _cache = cache;
            _mapper = mapper;
            _buscarRestaurante = buscarRestaurante;
            _agregarFavoritoUseCase = agregarUsuarioRestauranteFavoritoUseCase;
            _registrarTop3IndividualUseCase = registrarTop3IndividualUseCase;
            _registrarVisitaPerfilUseCase = registrarVisitaPerfilUseCase;
            _obtenerMetricasRestauranteUseCase = obtenerMetricasRestauranteUseCase;
            _actualizarRestauranteDashboardUseCase = actualizarRestauranteDashboardUseCase;
            _firebase = firebase;
            _obtenerRestauranteDetalle = obtenerRestauranteDetalle;
            _db = db;

        }

       
        [HttpGet]
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



        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var result = await _obtenerRestauranteDetalle.HandleAsync(id, uid, ct);

            var dto = _mapper.Map<RestauranteDetalleDto>(result.Restaurante);
            dto.esFavorito = result.EsFavorito;

            return Ok(dto);
        }




        [HttpPost]
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
                    dto.RestriccionesQueRespetaIds, imagenes, dto.WebsiteUrl, ct);

                return Ok(response);
            }
            catch (Exception ex)
            {
                foreach (var url in urlsSubidas)
                {
                    try { await _firebaseStorage.DeleteFileAsync(url); }
                    catch { }
                }

                return BadRequest(new { error = ex.Message });
            }
        }

      
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

        [Authorize(Policy = "DuenoRestaurante")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> ActualizarBasico(
            Guid id,
            [FromBody] ActualizarRestauranteDashboardRequest dto,
            CancellationToken ct)
        {

            var restauranteActualizado = await _actualizarRestauranteDashboardUseCase.HandleAsync(
                id,
                dto.Direccion,
                dto.Latitud,
                dto.Longitud,
                dto.HorariosJson,
                dto.WebUrl,
                dto.GustosQueSirveIds,
                dto.RestriccionesQueRespetaIds,
                ct);

            var detalle = _mapper.Map<RestauranteDetalleDto>(restauranteActualizado);
            return Ok(detalle);
        }


        private async Task ReemplazarImagenesTipoAsync(
            Guid restauranteId,
            TipoImagenRestaurante tipo,
            IList<IFormFile>? archivos,
            bool soloBorrar,
            List<string> urlsSubidas,
            CancellationToken ct)
        {
            var imagenesExistentes = await _db.RestauranteImagenes
                .Where(i => i.RestauranteId == restauranteId && i.Tipo == tipo)
                .ToListAsync(ct);

            foreach (var img in imagenesExistentes)
            {
                try
                {
                    await _firebase.DeleteFileAsync(img.Url);
                }
                catch
                {
                }

                _db.RestauranteImagenes.Remove(img);
            }

            if (soloBorrar || archivos == null || archivos.Count == 0)
                return;

            var orden = 0;
            foreach (var archivo in archivos)
            {
                using var stream = archivo.OpenReadStream();
                var url = await _firebase.UploadFileAsync(stream, archivo.FileName, "restaurantes");
                urlsSubidas.Add(url);

                var entidad = new RestauranteImagen
                {
                    RestauranteId = restauranteId,
                    Tipo = tipo,
                    Url = url,
                    Orden = orden++,
                    FechaCreacionUtc = DateTime.UtcNow
                };

                await _db.RestauranteImagenes.AddAsync(entidad, ct);
            }
        }



        [Authorize(Policy = "DuenoRestaurante")]
        [HttpGet("mio")]
        public async Task<IActionResult> ObtenerRestauranteIdDueñoRestaurante()
        {
            var firebaseuid= GetFirebaseUid();
            var usuario= await _obtenerUsuario.HandleAsync(FirebaseUid: firebaseuid, ct: CancellationToken.None);
            var restaurante = await _servicio.ObtenerPorPropietarioAsync(usuario.Id);

            return Ok(restaurante.Id);
        }

        [Authorize(Policy = "DuenoRestaurante")]
        [HttpPut("{id:guid}/imagenes/destacada")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ActualizarImagenDestacada(
            Guid id,
            [FromForm] ActualizarImagenRestauranteRequest request,
            CancellationToken ct = default)
        {

            var urlsSubidas = new List<string>();

            try
            {
                var restaurante = await _db.Restaurantes
                    .FirstOrDefaultAsync(r => r.Id == id, ct);

                if (restaurante == null)
                    return NotFound("Restaurante no encontrado.");

                if (!string.IsNullOrWhiteSpace(restaurante.ImagenUrl))
                {
                    try { await _firebase.DeleteFileAsync(restaurante.ImagenUrl); }
                    catch { }

                    restaurante.ImagenUrl = null;
                }

                if (!request.SoloBorrar && request.Archivo != null)
                {
                    using var stream = request.Archivo.OpenReadStream();
                    var url = await _firebase.UploadFileAsync(
                        stream,
                        request.Archivo.FileName,
                        "restaurantes");

                    urlsSubidas.Add(url);
                    restaurante.ImagenUrl = url;
                }

                restaurante.ActualizadoUtc = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);

                return Ok(new { imagenDestacada = restaurante.ImagenUrl });
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



        [Authorize(Policy = "DuenoRestaurante")]
        [HttpPut("{id:guid}/imagenes/logo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ActualizarLogo(
    Guid id,
    [FromForm] ActualizarImagenRestauranteRequest request,
    CancellationToken ct = default)
        {

            var urlsSubidas = new List<string>();

            try
            {
                var restaurante = await _db.Restaurantes
                    .FirstOrDefaultAsync(r => r.Id == id, ct);

                if (restaurante == null)
                    return NotFound("Restaurante no encontrado.");

                if (!string.IsNullOrWhiteSpace(restaurante.LogoUrl))
                {
                    try { await _firebase.DeleteFileAsync(restaurante.LogoUrl); }
                    catch { }

                    restaurante.LogoUrl = null;
                }

                if (!request.SoloBorrar && request.Archivo != null)
                {
                    using var stream = request.Archivo.OpenReadStream();
                    var url = await _firebase.UploadFileAsync(
                        stream,
                        request.Archivo.FileName,
                        "restaurantes");

                    urlsSubidas.Add(url);
                    restaurante.LogoUrl = url;
                }

                restaurante.ActualizadoUtc = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);

                return Ok(new { logoUrl = restaurante.LogoUrl });
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


        [Authorize(Policy = "DuenoRestaurante")]
        [HttpPut("{id:guid}/imagenes/interior")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ActualizarImagenesInterior(
    Guid id,
    [FromForm] ActualizarImagenesRestauranteRequest request,
    CancellationToken ct = default)
        {
            
            var urlsSubidas = new List<string>();

            try
            {
                var restaurante = await _db.Restaurantes
                    .FirstOrDefaultAsync(r => r.Id == id, ct);

                if (restaurante == null)
                    return NotFound("Restaurante no encontrado.");

                await ReemplazarImagenesTipoAsync(
                    restauranteId: id,
                    tipo: TipoImagenRestaurante.Interior,
                    archivos: request.Archivos,
                    soloBorrar: request.SoloBorrar,
                    urlsSubidas: urlsSubidas,
                    ct: ct);

                restaurante.ActualizadoUtc = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);

                var urls = await _db.RestauranteImagenes
                    .Where(i => i.RestauranteId == id && i.Tipo == TipoImagenRestaurante.Interior)
                    .OrderBy(i => i.Orden)
                    .Select(i => i.Url)
                    .ToListAsync(ct);

                return Ok(new { imagenesInterior = urls });
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



        [Authorize(Policy = "DuenoRestaurante")]
        [HttpPut("{id:guid}/imagenes/comidas")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ActualizarImagenesComida(
    Guid id,
    [FromForm] ActualizarImagenesRestauranteRequest request,
    CancellationToken ct = default)
        {

            var urlsSubidas = new List<string>();

            try
            {
                var restaurante = await _db.Restaurantes
                    .FirstOrDefaultAsync(r => r.Id == id, ct);

                if (restaurante == null)
                    return NotFound("Restaurante no encontrado.");

                await ReemplazarImagenesTipoAsync(
                    restauranteId: id,
                    tipo: TipoImagenRestaurante.Comida,
                    archivos: request.Archivos,
                    soloBorrar: request.SoloBorrar,
                    urlsSubidas: urlsSubidas,
                    ct: ct);

                restaurante.ActualizadoUtc = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);

                var urls = await _db.RestauranteImagenes
                    .Where(i => i.RestauranteId == id && i.Tipo == TipoImagenRestaurante.Comida)
                    .OrderBy(i => i.Orden)
                    .Select(i => i.Url)
                    .ToListAsync(ct);

                return Ok(new { imagenesComida = urls });
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


        [Authorize(Policy = "DuenoRestaurante")]
        [HttpPut("{id:guid}/imagenes/menu")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ActualizarImagenMenu(
    Guid id,
    [FromForm] ActualizarImagenRestauranteRequest request,
    CancellationToken ct = default)
        {

            var urlsSubidas = new List<string>();

            try
            {
                var restaurante = await _db.Restaurantes
                    .FirstOrDefaultAsync(r => r.Id == id, ct);

                if (restaurante == null)
                    return NotFound("Restaurante no encontrado.");

                IList<IFormFile>? archivos = null;
                if (request.Archivo != null)
                    archivos = new List<IFormFile> { request.Archivo };

                await ReemplazarImagenesTipoAsync(
                    restauranteId: id,
                    tipo: TipoImagenRestaurante.Menu,
                    archivos: archivos,
                    soloBorrar: request.SoloBorrar,
                    urlsSubidas: urlsSubidas,
                    ct: ct);

                restaurante.ActualizadoUtc = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);

                var urlMenu = await _db.RestauranteImagenes
                    .Where(i => i.RestauranteId == id && i.Tipo == TipoImagenRestaurante.Menu)
                    .OrderBy(i => i.Orden)
                    .Select(i => i.Url)
                    .FirstOrDefaultAsync(ct);

                return Ok(new { imagenMenu = urlMenu });
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

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var uid = GetFirebaseUid();


            var esAdmin = User.IsInRole("admin") || User.Claims.Any(c => c.Type == "role" && c.Value == "admin");

            var ok = await _servicio.EliminarAsync(id, uid, esAdmin);
            return ok ? NoContent() : NotFound();
        }

        
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string texto, CancellationToken ct)
        {
            var uid = GetFirebaseUid();

            var restaurantes = await _buscarRestaurante.HandleAsync(texto, ct);

            var dto = restaurantes.Select(r => new RestauranteResponse
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Latitud = r.Latitud,
                Longitud = r.Longitud,
                Categoria = r.Categoria,
                Rating = r.Rating,
                Direccion = r.Direccion,
                ImagenUrl = string.IsNullOrWhiteSpace(r.ImagenUrl) ? "https://firebasestorage.googleapis.com/v0/b/gustosapp-5c3c9.firebasestorage.app/o/restauranteicono.jpg?alt=media&token=cb818ad4-78b0-4fbb-a13d-46ce1aa66ac1" : r.ImagenUrl
            }).ToList();

            return Ok(dto);
        }


        [Authorize]
        [HttpPost("favorito/{restauranteId}")]
        public async Task<IActionResult> AgregarFavorito(Guid restauranteId)
        {
            var firebaseUid = GetFirebaseUid();
            await _agregarFavoritoUseCase.HandleAsync(firebaseUid, restauranteId);
            return Ok();
        }

        [HttpDelete("favorito/{restauranteId}")]
        public async Task<IActionResult> EliminarFavorito(Guid restauranteId)
        {
            var firebaseUid = GetFirebaseUid();
                await _agregarFavoritoUseCase.HandleAsyncDelete(firebaseUid, restauranteId);

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

        private async Task<SolicitudRestauranteImagen> SubirImagenAsync(IFormFile archivo,
       TipoImagenSolicitud tipo, List<string> urlsSubidas)

        {
            using var stream = archivo.OpenReadStream();
            var url = await _firebaseStorage.UploadFileAsync(stream, archivo.FileName, "solicitudes");
            urlsSubidas.Add(url);
            return new SolicitudRestauranteImagen
            {
                Tipo = tipo,
                Url = url
            };
        }
    }



}

