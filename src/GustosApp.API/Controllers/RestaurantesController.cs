using AutoMapper;
using GustosApp.API.DTO;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Interfaces;
using GustosApp.Application.Services;
using GustosApp.Application.UseCases.RestauranteUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases;
using GustosApp.Application.UseCases.UsuarioUseCases.GustoUseCases;
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
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


// Controlador para restaurantes que se registran en la app por un usuario y restaurantes traidos de Places v1

namespace GustosApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantesController : ControllerBase
    {
        private readonly IServicioRestaurantes _servicio;
        private readonly ObtenerGustosUseCase _obtenerGustos;
        private readonly SugerirGustosSobreUnRadioUseCase _sugerirGustos;
        private readonly BuscarRestaurantesCercanosUseCase _buscarRestaurantes;
        private readonly ActualizarDetallesRestauranteUseCase _obtenerDetalles;
        private readonly BuscarRestaurantesUseCase _buscarRestaurante;
        private readonly IAlmacenamientoArchivos _fileStorage;
        private readonly GustosApp.Infraestructure.GustosDbContext _db;
        private readonly IOcrService _ocr;
        private readonly IMenuParser _menuParser;
        private IMapper _mapper;
        private readonly AgregarUsuarioRestauranteFavoritoUseCase _agregarFavoritoUseCase;


        public RestaurantesController(
     IServicioRestaurantes servicio,
     SugerirGustosSobreUnRadioUseCase sugerirGustos,
     ObtenerGustosUseCase obtenerGustos,
     BuscarRestaurantesCercanosUseCase buscarRestaurantes,
     ActualizarDetallesRestauranteUseCase obtenerDetalles,
     IAlmacenamientoArchivos fileStorage,
     GustosApp.Infraestructure.GustosDbContext db,
     IOcrService ocr,
     IMenuParser menuParser,
     IMapper mapper, BuscarRestaurantesUseCase buscarRestaurante, AgregarUsuarioRestauranteFavoritoUseCase agregarUsuarioRestauranteFavoritoUseCase)
        {
            _servicio = servicio;
            _obtenerGustos = obtenerGustos;
            _sugerirGustos = sugerirGustos;
            _buscarRestaurantes = buscarRestaurantes;
            _obtenerDetalles = obtenerDetalles;
            _fileStorage = fileStorage;
            _db = db;
            _fileStorage = fileStorage;
            _mapper = mapper;

            _ocr = ocr;
            _menuParser = menuParser;
            _buscarRestaurante = buscarRestaurante;
            _agregarFavoritoUseCase = agregarUsuarioRestauranteFavoritoUseCase;
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
            CancellationToken ct,
            string? tipoDeRestaurante,
            [FromQuery] double rating,
            [FromQuery(Name = "near.lat")] double? lat = -34.641812775271,
            [FromQuery(Name = "near.lng")] double? lng = -58.56990230458638,
            [FromQuery(Name = "radiusMeters")] int? radius = 1000,
            [FromQuery] int top = 10
        )
        {
            var res = await _servicio.BuscarAsync(
                tipo: tipoDeRestaurante,
                plato: "",
                lat: lat,
                lng: lng,
                radioMetros: radius,
                rating: rating
            );

            Console.WriteLine(res.ToList().Count());
            Console.WriteLine(res.ToList().Count());
            Console.WriteLine(res.ToList().Count());


            var firebaseUid = User.FindFirst("user_id")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(firebaseUid))
            {
                return Unauthorized(new { message = "No se encontr el UID de Firebase en el token." });
            }

            var preferencias = await _obtenerGustos.HandleAsync(firebaseUid, ct, gustos);

            var recommendations = _sugerirGustos.Handle(preferencias, res, top, ct);

            var response = recommendations.Select(r => new RestauranteDTO
            {
                Id = r.Id,
                PropietarioUid = r.PropietarioUid,
                Nombre = r.Nombre,
                Direccion = r.Direccion,
                Latitud = r.Latitud,
                Longitud = r.Longitud,
                ImagenUrl = r.ImagenUrl,
                Rating = r.Rating ?? 0,
                Valoracion = r.Valoracion,
                Tipo = r.TypesJson,
                GustosQueSirve = r.GustosQueSirve
             .Select(g => new GustoDto(g.Id, g.Nombre, g.ImagenUrl))
             .ToList(),
                RestriccionesQueRespeta = r.RestriccionesQueRespeta
             .Select(re => new RestriccionResponse(re.Id, re.Nombre))
             .ToList(),
                Score = r.Score
            }).ToList();

            return Ok(new
            {
                total = response.Count,
                recomendaciones = response
            });
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var restaurante = await _servicio.ObtenerAsync(id);

            if (restaurante == null)
                return NotFound("Restaurante no encontrado");


            if (restaurante.PlaceId != null && (restaurante.Reviews == null || !restaurante.Reviews.Any()))
            {
                var actualizado = await _servicio.ObtenerResenasDesdeGooglePlaces(restaurante.PlaceId, ct);
                if (actualizado is not null)
                    restaurante = actualizado;
            }

            return Ok(restaurante);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CrearRestauranteDto dto, CancellationToken ct)
        {
            var uid = User.FindFirst("user_id")?.Value
                      ?? User.FindFirst("sub")?.Value
                      ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();

            var (lat, lng) = dto.Coordenadas;
            dto.Latitud = lat;
            dto.Longitud = lng;

            if (dto.Horarios is null)
            {
                var json = dto.HorariosComoJson;
                if (!string.IsNullOrWhiteSpace(json))
                {
                    dto.Horarios = JsonSerializer.Deserialize<JsonElement>(json);
                }
            }

            var creado = await _servicio.CrearAsync(uid, dto);

            var restaurante = await _db.Restaurantes
                .Include(r => r.GustosQueSirve)
                .Include(r => r.RestriccionesQueRespeta)
                .FirstOrDefaultAsync(r => r.Id == creado.Id, ct);

            if (restaurante is null)
                return CreatedAtAction(nameof(GetById), new { id = creado.Id }, creado);

            if (dto.GustosQueSirveIds is { Count: > 0 })
            {
                var gustos = await _db.Gustos
                    .Where(g => dto.GustosQueSirveIds!.Contains(g.Id))
                    .ToListAsync(ct);

                foreach (var g in gustos)
                {
                    if (!restaurante.GustosQueSirve.Any(x => x.Id == g.Id))
                        restaurante.GustosQueSirve.Add(g);
                }
            }

            if (dto.RestriccionesQueRespetaIds is { Count: > 0 })
            {
                var restricciones = await _db.Restricciones
                    .Where(r => dto.RestriccionesQueRespetaIds!.Contains(r.Id))
                    .ToListAsync(ct);

                foreach (var r in restricciones)
                {
                    if (!restaurante.RestriccionesQueRespeta.Any(x => x.Id == r.Id))
                        restaurante.RestriccionesQueRespeta.Add(r);
                }
            }

            await _db.SaveChangesAsync(ct);


            return CreatedAtAction(nameof(GetById), new { id = creado.Id }, new
            {
                creado.Id,
                creado.Nombre,
                creado.Direccion,
                creado.Latitud,
                creado.Longitud,
                PrimaryType = creado.PrimaryType,
                Types = creado.TypesJson,
                creado.ImagenUrl,
                GustosVinculados = dto.GustosQueSirveIds,
                RestriccionesVinculadas = dto.RestriccionesQueRespetaIds
            });
        }


        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Put(Guid id, [FromBody] ActualizarRestauranteDto dto)
        {
            var uid = User.FindFirst("user_id")?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(uid)) return Unauthorized();

            var esAdmin = User.IsInRole("admin") || User.Claims.Any(c => c.Type == "role" && c.Value == "admin");

            var r = await _servicio.ActualizarAsync(id, uid, esAdmin, dto);
            return r is null ? NotFound() : Ok(r);
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

        [Authorize]
        [HttpPost("{id:guid}/imagenes/{tipo}")]
        public async Task<IActionResult> SubirImagen(Guid id, string tipo, IFormFile archivo, CancellationToken ct)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest(new { mensaje = "Archivo requerido" });

            var (ok, uid) = await CheckOwnerAsync(id, User, ct);
            if (!ok) return Forbid();

            if (!Enum.TryParse<TipoImagenRestaurante>(
                tipo.Replace("-", "", StringComparison.OrdinalIgnoreCase), ignoreCase: true, out var tipoImg))
            {
                tipo = tipo.ToLowerInvariant();
                if (tipo == "perfil") tipoImg = TipoImagenRestaurante.Perfil;
                else if (tipo == "principal") tipoImg = TipoImagenRestaurante.Principal;
                else if (tipo == "interior") tipoImg = TipoImagenRestaurante.Interior;
                else if (tipo == "comida") tipoImg = TipoImagenRestaurante.Comida;
                else return BadRequest(new { mensaje = "Tipo de imagen inválido" });
            }

            if (!(archivo.ContentType?.StartsWith("image/") ?? false))
                return BadRequest(new { mensaje = "Tipo de archivo no soportado" });

            var ext = Path.GetExtension(archivo.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var rutaRel = Path.Combine(id.ToString(), tipoImg.ToString().ToLowerInvariant(), fileName);
            using var s = archivo.OpenReadStream();
            var url = await _fileStorage.SubirAsync(s, rutaRel, ct);

            var reg = new RestauranteImagen
            {
                RestauranteId = id,
                Tipo = tipoImg,
                Url = url,
                FechaCreacionUtc = DateTime.UtcNow
            };
            _db.Set<RestauranteImagen>().Add(reg);
            await _db.SaveChangesAsync(ct);

            return Created(url, new { mensaje = "Imagen subida", url });
        }

        [Authorize]
        [HttpPost("{id:guid}/menu/manual")]
        [Consumes("application/json")]
        public async Task<IActionResult> GuardarMenuManual(Guid id, [FromBody] object menu, CancellationToken ct)
        {
            var (ok, uid) = await CheckOwnerAsync(id, User, ct);
            if (!ok) return Forbid();

            if (menu is null) return BadRequest(new { mensaje = "menu requerido" });

            // Validación real del JSON y default de moneda
            using var doc = System.Text.Json.JsonDocument.Parse(JsonSerializer.Serialize(menu));
            var root = doc.RootElement;

            if (!root.TryGetProperty("categorias", out var categorias) || categorias.ValueKind != JsonValueKind.Array)
                return BadRequest(new { mensaje = "El menú debe contener 'categorias' como arreglo." });

            var moneda = root.TryGetProperty("moneda", out var monedaEl) && monedaEl.ValueKind == JsonValueKind.String
                ? monedaEl.GetString()
                : "ARS";

            var json = JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            var repo = _db.Set<RestauranteMenu>();
            var existente = await repo.FirstOrDefaultAsync(m => m.RestauranteId == id, ct);

            var created = false;
            if (existente == null)
            {
                existente = new RestauranteMenu
                {
                    RestauranteId = id,
                    Moneda = moneda ?? "ARS",
                    Json = json,
                    Version = 1,
                    FechaActualizacionUtc = DateTime.UtcNow
                };
                repo.Add(existente);
                created = true;
            }
            else
            {
                existente.Moneda = moneda ?? existente.Moneda;
                existente.Json = json;
                existente.Version += 1;
                existente.FechaActualizacionUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(ct);

            if (created)
            {

                return CreatedAtAction(nameof(GetMenu), new { id }, new
                {
                    mensaje = "Menú creado",
                    version = existente.Version
                });
            }
            return Ok(new { mensaje = "Menú actualizado", version = existente.Version });
        }


        [Authorize]
        [HttpPost("{id:guid}/menu/ocr")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> MenuOCR(Guid id, [FromForm] List<IFormFile> archivos, CancellationToken ct)
        {
            var (ok, uid) = await CheckOwnerAsync(id, User, ct);
            if (!ok) return Forbid();

            if (archivos is null || archivos.Count == 0)
                return BadRequest(new { mensaje = "Debe enviar al menos una imagen" });

            if (archivos.Count > 10)
                return BadRequest(new { mensaje = "Máximo 10 imágenes por solicitud" });

            const long MaxFileSizeBytes = 10 * 1024 * 1024;
            var ocrStreams = new List<Stream>();

            try
            {
                foreach (var a in archivos)
                {
                    if (!(a.ContentType?.StartsWith("image/") ?? false))
                        return BadRequest(new { mensaje = $"Tipo de archivo no soportado: {a.FileName}" });

                    if (a.Length <= 0 || a.Length > MaxFileSizeBytes)
                        return BadRequest(new { mensaje = $"Archivo inválido o excede el límite ({a.FileName})." });

                    using var tmp = new MemoryStream();
                    await a.OpenReadStream().CopyToAsync(tmp, ct);
                    var bytes = tmp.ToArray();

                    var ext = Path.GetExtension(a.FileName);
                    var nombre = $"{Guid.NewGuid():N}{ext}";
                    var rutaRel = Path.Combine(id.ToString(), "menu", nombre);

                    using (var sStore = new MemoryStream(bytes, writable: false))
                        await _fileStorage.SubirAsync(sStore, rutaRel, ct);

                    ocrStreams.Add(new MemoryStream(bytes, writable: false));
                }

                var textoReconocido = await _ocr.ReconocerTextoAsync(ocrStreams, "spa+eng", ct);
                if (string.IsNullOrWhiteSpace(textoReconocido))
                    return BadRequest(new { mensaje = "No se pudo extraer texto del menú" });

                var menuJson = await _menuParser.ParsearAsync(textoReconocido, "ARS", ct);

                var repo = _db.Set<RestauranteMenu>();
                var existente = await repo.FirstOrDefaultAsync(m => m.RestauranteId == id, ct);
                var created = false;

                if (existente == null)
                {
                    existente = new RestauranteMenu
                    {
                        RestauranteId = id,
                        Moneda = "ARS",
                        Json = menuJson,
                        Version = 1,
                        FechaActualizacionUtc = DateTime.UtcNow
                    };
                    repo.Add(existente);
                    created = true;
                }
                else
                {
                    existente.Json = menuJson;
                    existente.Version += 1;
                    existente.FechaActualizacionUtc = DateTime.UtcNow;
                }

                await _db.SaveChangesAsync(ct);

                JsonElement menuElem;
                using (var doc = System.Text.Json.JsonDocument.Parse(menuJson))
                    menuElem = doc.RootElement.Clone();

                var payload = new
                {
                    mensaje = created ? "Menú OCR creado" : "Menú OCR actualizado",
                    version = existente.Version,
                    menu = menuElem
                };

                return created
                    ? CreatedAtAction(nameof(GetMenu), new { id }, payload)
                    : Ok(payload);
            }
            finally
            {
                foreach (var s in ocrStreams) s.Dispose();
            }
        }


        // ---------------------------------------------
        // GET /api/Restaurantes/{id}/imagenes?tipo=perfil|principal|interior|comida|menu&skip=0&take=20&miniatura=true
        // ---------------------------------------------
        [HttpGet("{id:guid}/imagenes")]
        public async Task<ActionResult<ImagenesResponse>> GetImagenes(
            Guid id,
            [FromQuery] string? tipo,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            [FromQuery] bool miniatura = false,
            CancellationToken ct = default)
        {
            var existe = await _db.Restaurantes.AsNoTracking().AnyAsync(r => r.Id == id, ct);
            if (!existe) return NotFound(new { mensaje = "Restaurante no encontrado" });

            TipoImagenRestaurante? tipoImg = null;
            if (!string.IsNullOrWhiteSpace(tipo))
            {
                if (!TryParseTipo(tipo, out var parsed))
                    return BadRequest(new { mensaje = "Tipo de imagen inválido. Use perfil|principal|interior|comida|menu" });
                tipoImg = parsed;
            }

            var q = _db.Set<RestauranteImagen>()
                .AsNoTracking()
                .Where(i => i.RestauranteId == id);

            if (tipoImg.HasValue)
                q = q.Where(i => i.Tipo == tipoImg.Value);

            q = q.OrderBy(i => i.Orden == null)
                 .ThenBy(i => i.Orden)
                 .ThenByDescending(i => i.FechaCreacionUtc);

            var total = await q.CountAsync(ct);

            if (take <= 0) take = 20;
            if (skip < 0) skip = 0;

            var data = await q.Skip(skip).Take(take)
                .Select(i => new RestauranteImagenDto
                {
                    Id = i.Id,
                    Tipo = i.Tipo.ToString().ToLowerInvariant(),
                    Url = i.Url,
                    Orden = i.Orden,
                    FechaCreacionUtc = i.FechaCreacionUtc,
                    MiniaturaUrl = (miniatura && i.Tipo == TipoImagenRestaurante.Perfil)
                        ? i.Url
                        : null
                })
                .ToListAsync(ct);

            return Ok(new ImagenesResponse
            {
                Total = total,
                Skip = skip,
                Take = take,
                Items = data
            });
        }

        // ---------------------------------------------
        // GET /api/Restaurantes/{id}/imagenes/{imagenId}
        // ---------------------------------------------
        [HttpGet("{id:guid}/imagenes/{imagenId:guid}")]
        public async Task<ActionResult<RestauranteImagenDto>> GetImagenPorId(
            Guid id,
            Guid imagenId,
            [FromQuery] bool miniatura = false,
            CancellationToken ct = default)
        {
            var img = await _db.Set<RestauranteImagen>()
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.RestauranteId == id && i.Id == imagenId, ct);

            if (img is null) return NotFound(new { mensaje = "Imagen no encontrada" });

            var dto = new RestauranteImagenDto
            {
                Id = img.Id,
                Tipo = img.Tipo.ToString().ToLowerInvariant(),
                Url = img.Url,
                Orden = img.Orden,
                FechaCreacionUtc = img.FechaCreacionUtc,
                MiniaturaUrl = (miniatura && img.Tipo == TipoImagenRestaurante.Perfil)
                    ? img.Url /* o tu convención de thumbs */
                    : null
            };

            return Ok(dto);
        }

        // ---------------------------------------------
        // GET /api/Restaurantes/{id}/menu
        // ---------------------------------------------
        [HttpGet("{id:guid}/menu")]
        public async Task<ActionResult<RestauranteMenuDto>> GetMenu(Guid id, CancellationToken ct = default)
        {
            var menu = await _db.Set<RestauranteMenu>()
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.RestauranteId == id, ct);

            if (menu is null) return NotFound(new { mensaje = "El restaurante no tiene menú guardado" });


            JsonElement? parsed = null;
            try
            {
                using var doc = JsonDocument.Parse(menu.Json ?? "{}");
                parsed = doc.RootElement.Clone();
            }
            catch
            {

            }

            var dto = new RestauranteMenuDto
            {
                Id = menu.Id,
                RestauranteId = menu.RestauranteId,
                Moneda = menu.Moneda,
                Version = menu.Version,
                FechaActualizacionUtc = menu.FechaActualizacionUtc,
                Menu = parsed,
                MenuRaw = parsed is null ? (menu.Json ?? "{}") : null
            };

            return Ok(dto);
        }

        // -------------------
        // Helpers
        // -------------------
        private static bool TryParseTipo(string tipo, out TipoImagenRestaurante value)
        {

            var t = tipo.Trim().ToLowerInvariant();
            switch (t)
            {
                case "perfil": value = TipoImagenRestaurante.Perfil; return true;
                case "principal": value = TipoImagenRestaurante.Principal; return true;
                case "interior": value = TipoImagenRestaurante.Interior; return true;
                case "comida": value = TipoImagenRestaurante.Comida; return true;
                case "menu": value = TipoImagenRestaurante.Menu; return true;
                default:

                    return Enum.TryParse<TipoImagenRestaurante>(tipo, true, out value);
            }
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

    }

}

