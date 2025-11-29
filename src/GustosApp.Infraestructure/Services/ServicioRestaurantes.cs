using Google.Protobuf.WellKnownTypes;
using GustosApp.Application.DTO;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using GustosApp.Infraestructure.Extrerno.GooglePlacesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;


namespace GustosApp.Infraestructure.Services
{
    public class ServicioRestaurantes : IServicioRestaurantes
    {
        private readonly GustosDbContext _db;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;
        private readonly IRestauranteRepository _repo;

        public ServicioRestaurantes(GustosDbContext db,
        IConfiguration config, HttpClient http, IRestauranteRepository repo)
        {
            _db = db;
            _config = config;
            _http = http;
            _repo = repo;
        }

        private static string NormalizarNombre(string nombre)
            => (nombre ?? string.Empty).Trim().ToLowerInvariant();

        public async Task<List<Restaurante>> BuscarAsync(
     double rating,
     string? tipo,
     string? plato,
     double? lat = null,
     double? lng = null,
     int? radioMetros = null)
        {
            IQueryable<Restaurante> query = _db.Restaurantes
                .AsNoTracking()
                .Include(r => r.GustosQueSirve)
                .Include(r => r.RestriccionesQueRespeta)
                .Include(r => r.Platos);

            // 1️⃣ FILTRAR POR TIPO DIRECTO EN SQL (si se puede mejorar después)
            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var t = tipo.Trim().ToLower();
                query = query.Where(r =>
                    r.TypesJson != null &&
                    EF.Functions.Like(r.TypesJson.ToLower(), $"%\"{t}\"%"));
            }

            // 2️⃣ FILTRO GEOGRÁFICO USANDO SQL
            if (lat.HasValue && lng.HasValue && radioMetros.HasValue && radioMetros.Value > 0)
            {
                double latVal = lat.Value;
                double lngVal = lng.Value;

                double degLat = radioMetros.Value / 111_000.0;
                double degLng = radioMetros.Value / (111_000.0 * Math.Cos(latVal * Math.PI / 180.0));

                double minLat = latVal - degLat;
                double maxLat = latVal + degLat;
                double minLng = lngVal - degLng;
                double maxLng = lngVal + degLng;

                query = query.Where(r =>
                    r.Latitud >= minLat && r.Latitud <= maxLat &&
                    r.Longitud >= minLng && r.Longitud <= maxLng &&
                    r.Rating >= rating);

                // Orden + Take también en SQL
                query = query.OrderBy(r =>
                    Math.Abs(r.Latitud - latVal) + Math.Abs(r.Longitud - lngVal))
                    .Take(200);
            }
            else
            {
                query = query.Where(r => r.Rating >= rating)
                    .OrderBy(r => r.NombreNormalizado)
                    .Take(1000);
            }

            return await query.ToListAsync();
        }



       /* public async Task<Restaurante> CrearAsync(string propietarioUid, CrearRestauranteDto dto)
        {
            var nombreNorm = NormalizarNombre(dto.Nombre);
            var nombreEnUso = await _db.Restaurantes.AsNoTracking()
                .AnyAsync(r => r.NombreNormalizado == nombreNorm);
            if (nombreEnUso)
                throw new ArgumentException("El nombre ya está en uso.");

            var primaryType = string.IsNullOrWhiteSpace(dto.PrimaryType)
                ? "restaurant"
                : dto.PrimaryType!.Trim();

            var typesList = (dto.Types ?? new())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct()
                .ToList();

            var platosParsed = (dto.Platos ?? new())
                .Select(s =>
                {
                    if (!System.Enum.TryParse<PlatoComida>(s, true, out var p))
                        throw new ArgumentException($"Plato inválido: {s}");
                    return p;
                })
                .Distinct()
                .ToList();

            var ahora = DateTime.UtcNow;

            if (dto.Valoracion is null)
            {
                dto.Valoracion = 0;
            }

            var (latOpt, lngOpt) = dto.Coordenadas;
            if (latOpt is null || lngOpt is null)
                throw new ArgumentException("Lat/Lng requeridos en el cuerpo (usa 'lat' y 'lng' o 'latitud' y 'longitud')."); 
            
            var entidad = new Restaurante
                {
                    Id = Guid.NewGuid(),
                    PropietarioUid = propietarioUid,
                    Nombre = dto.Nombre.Trim(),
                    NombreNormalizado = nombreNorm,
                    Direccion = dto.Direccion.Trim(),
                    Latitud = latOpt.Value,
                    Longitud = lngOpt.Value,
                    HorariosJson = dto.HorariosComoJson ?? "{}",
                    CreadoUtc = ahora,
                    ActualizadoUtc = ahora,
                    PrimaryType = primaryType,
                    TypesJson = JsonSerializer.Serialize(typesList),
                    ImagenUrl = string.IsNullOrWhiteSpace(dto.ImagenUrl) ? null : dto.ImagenUrl!.Trim(),
                    Valoracion = (decimal?)dto.Valoracion,
                    Rating= dto.Valoracion,
                    CantidadResenas = 0
            };

            foreach (var p in platosParsed)
                entidad.Platos.Add(new RestaurantePlato { RestauranteId = entidad.Id, Plato = p });

            _db.Restaurantes.Add(entidad);
            await _db.SaveChangesAsync();
            await _db.Entry(entidad).Collection(x => x.Platos).LoadAsync();

            return entidad;
        }

        */
        public async Task<Restaurante?> ObtenerAsync(Guid id)
        {
            var r = await _db.Restaurantes

         .Include(r => r.Reviews)
          .ThenInclude(o => o.Usuario)
         .Include(r => r.Reviews)
           .ThenInclude(o => o.Fotos)

         .Include(r => r.Imagenes)

         .Include(r => r.Menu)

         // ================================
         // PLATOS (si los usás en otra parte)
         // ================================
         //.Include(r => r.Platos)

         .FirstOrDefaultAsync(r => r.Id == id);

            return r;
        }


        public async Task<Restaurante?> ObtenerPorPropietarioAsync(Guid DuenoID)
        {
            var r = await _db.Restaurantes.AsNoTracking()
                .FirstOrDefaultAsync(x => x.DuenoId == DuenoID);
            return r is null ? null : r;
        }

        public async Task<IReadOnlyList<Restaurante>> ListarCercanosAsync(
            double lat, double lng, int radioMetros,
            string? tipo = null,
            IEnumerable<string>? platos = null)
        {
            double degLat = radioMetros / 111_000.0;
            double degLng = radioMetros / (111_000.0 * Math.Cos(lat * Math.PI / 180.0));

            var minLat = lat - degLat;
            var maxLat = lat + degLat;
            var minLng = lng - degLng;
            var maxLng = lng + degLng;

            var q = _db.Restaurantes.AsNoTracking()
                .Include(r => r.Platos)
                .Where(r => r.Latitud >= minLat && r.Latitud <= maxLat
                         && r.Longitud >= minLng && r.Longitud <= maxLng);

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                var t = tipo.Trim();
                q = q.Where(r => r.PrimaryType == t || r.TypesJson.Contains($"\"{t}\""));
            }

            if (platos is not null)
            {
                var set = new HashSet<PlatoComida>(
                    platos.Select(p => System.Enum.TryParse<PlatoComida>(p, true, out var v) ? v : (PlatoComida?)null)
                          .Where(v => v.HasValue)
                          .Select(v => v!.Value));
                if (set.Count > 0)
                {
                    q = q.Where(r => r.Platos.Any(p => set.Contains(p.Plato)));
                }
            }

            var lista = await q
                .OrderBy(r => Math.Abs(r.Latitud - lat) + Math.Abs(r.Longitud - lng))
                .Take(200)
                .ToListAsync();

            return lista.ToList();
        }


        public async Task<bool> EliminarAsync(Guid id, string solicitanteUid, bool esAdmin)
        {
            var r = await _db.Restaurantes.FirstOrDefaultAsync(x => x.Id == id);
            if (r is null) return false;

            if (!esAdmin && r.PropietarioUid != solicitanteUid)
                throw new UnauthorizedAccessException("No tenés permisos para eliminar este restaurante.");

            _db.Restaurantes.Remove(r);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Restaurante> ObtenerResenasDesdeGooglePlaces(string placeId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(placeId))
                throw new ArgumentException("placeId requerido", nameof(placeId));

            var apiKey = _config["GooglePlaces:ApiKey"]
                ?? throw new InvalidOperationException("Falta GooglePlaces:ApiKey");

            var fieldMask = string.Join(",",
                "id,displayName,primaryType,types,priceLevel,rating,userRatingCount,",
                "formattedAddress,internationalPhoneNumber,websiteUri,location,photos.name,",
                "currentOpeningHours,reviews"
            );

            var url = $"https://places.googleapis.com/v1/places/{Uri.EscapeDataString(placeId)}?languageCode=es";

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.TryAddWithoutValidation("X-Goog-Api-Key", apiKey);
            req.Headers.TryAddWithoutValidation("X-Goog-FieldMask", fieldMask);

            var resp = await _http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
            {
                Console.WriteLine($" Google Places API error: {resp.StatusCode}");
                return await _repo.GetByPlaceIdAsync(placeId, ct)
                    ?? new Restaurante { PlaceId = placeId, Nombre = "(sin datos)" };
            }

            var details = await resp.Content.ReadFromJsonAsync<PlaceDetails>(cancellationToken: ct);
            if (details is null)
                return await _repo.GetByPlaceIdAsync(placeId, ct)
                    ?? new Restaurante { PlaceId = placeId, Nombre = "(sin datos)" };

            // Buscar restaurante existente
            var existente = await _repo.GetByPlaceIdAsync(placeId, ct);

            if (existente is null)
            {
                existente = new Restaurante
                {
                    Id = Guid.NewGuid(),
                    PropietarioUid = string.Empty,
                    Nombre = details.DisplayName?.Text ?? "(sin nombre)",
                    NombreNormalizado = (details.DisplayName?.Text ?? string.Empty).Trim().ToLowerInvariant(),
                    Direccion = details.FormattedAddress ?? string.Empty,
                    Latitud = details.Location?.Latitude ?? 0,
                    Longitud = details.Location?.Longitude ?? 0,
                    PlaceId = placeId,
                    Rating = details.Rating,
                    CantidadResenas = details.UserRatingCount,
                    Categoria = details.PrimaryType ?? "restaurant",
                    UltimaActualizacion = DateTime.UtcNow,
                    ImagenUrl = GetPhotoUrl(details.Photos?.FirstOrDefault()?.Name, apiKey),
                    Reviews = new List<OpinionRestaurante>()
                };

                await _repo.AddAsync(existente, ct);
            }
            else
            {
                // Actualizar información básica del restaurante
                existente.Nombre = details.DisplayName?.Text ?? existente.Nombre;
                existente.Direccion = details.FormattedAddress ?? existente.Direccion;
                existente.Latitud = details.Location?.Latitude ?? existente.Latitud;
                existente.Longitud = details.Location?.Longitude ?? existente.Longitud;
                existente.Rating = details.Rating ?? existente.Rating;
                existente.CantidadResenas = details.UserRatingCount ?? existente.CantidadResenas;
                existente.Categoria = details.PrimaryType ?? existente.Categoria;
                existente.ImagenUrl = GetPhotoUrl(details.Photos?.FirstOrDefault()?.Name, apiKey);
                existente.ActualizadoUtc = DateTime.UtcNow;
                existente.UltimaActualizacion = DateTime.UtcNow;
            }

            // Procesar opiniones desde Google
            if (details.Reviews is { Count: > 0 })
            {
                var opiniones = details.Reviews.Select(r => new OpinionRestaurante
                {
                    Id = Guid.NewGuid(),
                    RestauranteId = existente.Id,
                    UsuarioId = null, // no aplica para Google
                    AutorExterno = r.AuthorAttribution?.DisplayName ?? "Anónimo",
                    FuenteExterna = "GooglePlaces",
                    ImagenAutorExterno = r.AuthorAttribution?.PhotoUri,
                    Valoracion = r.Rating ,
                    Titulo = "Opinión desde Google",
                    Opinion = r.Text?.Text ?? "",
                    EsImportada = true,
                    FechaCreacion = DateTime.UtcNow
                }).ToList();

                // Evitar duplicados (comparando texto + autor)
                var existentes = await _db.OpinionesRestaurantes
                    .Where(x => x.RestauranteId == existente.Id && x.EsImportada)
                    .Select(x => new { x.AutorExterno, x.Opinion })
                    .ToListAsync(ct);

                var nuevas = opiniones
                    .Where(o => !existentes.Any(e =>
                        string.Equals(e.AutorExterno, o.AutorExterno, StringComparison.OrdinalIgnoreCase) &&
                        string.Equals(e.Opinion, o.Opinion, StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (nuevas.Any())
                {
                    await _db.OpinionesRestaurantes.AddRangeAsync(nuevas, ct);
                    await _db.SaveChangesAsync(ct);

                   
                    foreach (var nueva in nuevas)
                    {
                        existente.Reviews.Add(nueva);
                    }
                }
            }

            await _db.SaveChangesAsync(ct);
            return existente;
        }

        private static string? GetPhotoUrl(string? fotoName, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(fotoName)) return null;
            return $"https://places.googleapis.com/v1/{Uri.EscapeDataString(fotoName)}/media?maxWidthPx=800&key={apiKey}";
        }

        public async Task<Application.DTO.GooglePlacesDto?> ObtenerMetricasGooglePlaces(string placeId, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(placeId))
            {
                Console.WriteLine("[ObtenerMetricasGooglePlaces] PlaceId está vacío o null");
                return null;
            }

            var apiKey = _config["GooglePlaces:ApiKey"]
                ?? throw new InvalidOperationException("Falta GooglePlaces:ApiKey");

            // Solo se necesita tener el rating y userRatingCount
            var fieldMask = "rating,userRatingCount";

            var url = $"https://places.googleapis.com/v1/places/{Uri.EscapeDataString(placeId)}?languageCode=es";

            // Función helper para intentar obtener desde BD
            async Task<Application.DTO.GooglePlacesDto?> TryGetFromDatabase()
            {
                var restauranteBD = await _repo.GetByPlaceIdAsync(placeId, ct);
                if (restauranteBD != null)
                {
                    var rating = restauranteBD.Rating ?? 0;
                    var totalRatings = restauranteBD.CantidadResenas ?? 0;
                    
                    Console.WriteLine($"   Metricas obtenidas desde BD para '{placeId}': Rating={rating}, TotalRatings={totalRatings}");
                    return new Application.DTO.GooglePlacesDto
                    {
                        Rating = rating,
                        TotalRatings = totalRatings
                    };
                }
                return null;
            }

            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                req.Headers.TryAddWithoutValidation("X-Goog-Api-Key", apiKey);
                req.Headers.TryAddWithoutValidation("X-Goog-FieldMask", fieldMask);

                var resp = await _http.SendAsync(req, ct);
                
                if (!resp.IsSuccessStatusCode)
                {
                    var errorContent = await resp.Content.ReadAsStringAsync(ct);
                    Console.WriteLine($"[ObtenerMetricasGooglePlaces] Error de Google Places API:");
                    Console.WriteLine($"  Status: {resp.StatusCode}");
                    Console.WriteLine($"  PlaceId usado: {placeId}");
                    Console.WriteLine($"  Error: {errorContent}");
                    
                    // Intentar desde BD en CUALQUIER error, no solo 404
                    Console.WriteLine($" Intentando obtener métricas desde base de datos como fallback...");
                    var metricsFromDB = await TryGetFromDatabase();
                    if (metricsFromDB != null)
                    {
                        return metricsFromDB;
                    }
                    
                    Console.WriteLine($"  ❌ No se encontraron métricas ni en Google Places ni en la base de datos para PlaceId: {placeId}");
                    return null;
                }

                var details = await resp.Content.ReadFromJsonAsync<PlaceDetails>(cancellationToken: ct);
                
                if (details is null)
                {
                    Console.WriteLine($"[ObtenerMetricasGooglePlaces] La respuesta de Google Places no se pudo deserializar para PlaceId: {placeId}");
                    
                    // Fallback: intentar desde BD
                    var metricsFromDB = await TryGetFromDatabase();
                    if (metricsFromDB != null)
                    {
                        return metricsFromDB;
                    }
                    
                    return null;
                }

                if (!details.Rating.HasValue)
                {
                    Console.WriteLine($"[ObtenerMetricasGooglePlaces] El lugar '{placeId}' no tiene rating disponible en Google Places. Intentando desde BD...");
                    
                    // Fallback: intentar desde BD
                    var metricsFromDB = await TryGetFromDatabase();
                    if (metricsFromDB != null)
                    {
                        return metricsFromDB;
                    }
                    
                    return null;
                }

                var result = new Application.DTO.GooglePlacesDto
                {
                    Rating = details.Rating.Value,
                    TotalRatings = details.UserRatingCount ?? 0
                };

                Console.WriteLine($"[ObtenerMetricasGooglePlaces]  Métricas obtenidas desde Google Places para '{placeId}': Rating={result.Rating}, TotalRatings={result.TotalRatings}");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ObtenerMetricasGooglePlaces] Excepción al obtener métricas de Google Places para '{placeId}': {ex.Message}");
                Console.WriteLine($"  StackTrace: {ex.StackTrace}");
                
                // Si hay cualquier excepcion, intentar desde BD como ultimo recurso
                Console.WriteLine($"   Intentando obtener métricas desde base de datos como fallback...");
                var metricsFromDB = await TryGetFromDatabase();
                if (metricsFromDB != null)
                {
                    return metricsFromDB;
                }
                
                return null;
            }
        }
    }
}
