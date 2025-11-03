using System.Net.Http.Json;
using System.Text.Json;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Services;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure;
using GustosApp.Infraestructure.Extrerno.GooglePlacesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


public class ServicioRestaurantes : IServicioRestaurantes
{
    private readonly GustosDbContext _db;
    private readonly IConfiguration _config;
    private readonly HttpClient _http;
    private readonly IRestauranteRepository _repo;


    public ServicioRestaurantes(GustosDbContext db,
        IConfiguration config,HttpClient http, IRestauranteRepository repo)
    {
        _db = db;
        _config = config;
        _http = http;
        _repo = repo;
    }

    private static string NormalizarNombre(string nombre)
        => (nombre ?? string.Empty).Trim().ToLowerInvariant();

 
    public async Task<List<Restaurante>> BuscarAsync(
        string? tipo,
        string? plato,
        double? lat = null,
        double? lng = null,
        int? radioMetros = null)
    {
        var q = _db.Restaurantes
            .Include(r => r.Platos)
            .Include(r => r.GustosQueSirve)
            .Include(r => r.RestriccionesQueRespeta)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            if (Enum.TryParse<TipoRestaurante>(tipo, true, out var tipoEnum))
                q = q.Where(r => r.Tipo == tipoEnum);
        }

        if (!string.IsNullOrWhiteSpace(plato))
        {
            q = q.Where(r => r.Platos.Any(p => p.Plato.ToString().Contains(plato)));
        }

        if (lat.HasValue && lng.HasValue && radioMetros.HasValue && radioMetros.Value > 0)
        {
            double degLat = radioMetros.Value / 111_000.0;
            double degLng = radioMetros.Value / (111_000.0 * Math.Cos(lat.Value * Math.PI / 180.0));

            var minLat = lat.Value - degLat;
            var maxLat = lat.Value + degLat;
            var minLng = lng.Value - degLng;
            var maxLng = lng.Value + degLng;

            q = q.Where(r =>
                r.Latitud >= minLat && r.Latitud <= maxLat &&
                r.Longitud >= minLng && r.Longitud <= maxLng);
        }

        return await q
            .OrderBy(r => r.NombreNormalizado)
            .Take(500)
            .ToListAsync();
    }

    public async Task<Restaurante> CrearAsync(string propietarioUid, CrearRestauranteDto dto)
    {
        var nombreNorm = NormalizarNombre(dto.Nombre);

        if (await _db.Restaurantes.AnyAsync(r => r.NombreNormalizado == nombreNorm))
            throw new ArgumentException("El nombre ya está en uso.");

        var entidad = new Restaurante
        {
            Id = Guid.NewGuid(),
            PropietarioUid = propietarioUid,
            Nombre = dto.Nombre.Trim(),
            NombreNormalizado = nombreNorm,
            Direccion = dto.Direccion.Trim(),
            Latitud = (double)dto.Lat,
            Longitud = (double)dto.Longitud,
            HorariosJson = dto.Horarios is null ? "{}" : JsonSerializer.Serialize(dto.Horarios),
            CreadoUtc = DateTime.UtcNow,
            ActualizadoUtc = DateTime.UtcNow,
            Tipo = Enum.TryParse<TipoRestaurante>(dto.Tipo, true, out var tipoEnum)
                ? tipoEnum
                : TipoRestaurante.Restaurante,
            ImagenUrl = dto.ImagenUrl,
            Valoracion = (decimal?)dto.Valoracion
        };

        _db.Restaurantes.Add(entidad);
        await _db.SaveChangesAsync();

        return entidad;
    }

    public async Task<Restaurante?> ObtenerAsync(Guid id)
    {
        return await _db.Restaurantes
            .Include(r => r.Platos)
            .Include(r => r.GustosQueSirve)
            .Include(r => r.RestriccionesQueRespeta)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Restaurante?> ObtenerPorPropietarioAsync(string propietarioUid)
    {
        return await _db.Restaurantes
            .Include(r => r.Platos)
            .Include(r => r.GustosQueSirve)
            .Include(r => r.RestriccionesQueRespeta)
            .FirstOrDefaultAsync(r => r.PropietarioUid == propietarioUid);
    }

    public async Task<Restaurante?> ActualizarAsync(Guid id, string solicitanteUid, bool esAdmin, ActualizarRestauranteDto dto)
    {
        var r = await _db.Restaurantes
            .Include(x => x.Platos)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (r is null) return null;

        if (!esAdmin && r.PropietarioUid != solicitanteUid)
            throw new UnauthorizedAccessException("No tenés permisos para modificar este restaurante.");

        r.Nombre = dto.Nombre.Trim();
        r.NombreNormalizado = NormalizarNombre(dto.Nombre);
        r.Direccion = dto.Direccion.Trim();
        r.Latitud = (double)dto.Latitud;
        r.Longitud = (double)dto.Longitud;
        r.HorariosJson = dto.Horarios is null ? "{}" : JsonSerializer.Serialize(dto.Horarios);
        r.ActualizadoUtc = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.Tipo) &&
            Enum.TryParse<TipoRestaurante>(dto.Tipo, true, out var tipoEnum))
        {
            r.Tipo = tipoEnum;
        }

        r.ImagenUrl = dto.ImagenUrl;
        r.Valoracion = dto.Valoracion;

        await _db.SaveChangesAsync();
        return r;
    }

    public async Task<bool> EliminarAsync(Guid id, string solicitanteUid, bool esAdmin)
    {
        var r = await _db.Restaurantes.FindAsync(id);
        if (r is null) return false;

        if (!esAdmin && r.PropietarioUid != solicitanteUid)
            throw new UnauthorizedAccessException("No tenés permisos para eliminar este restaurante.");

        _db.Restaurantes.Remove(r);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Restaurante>> ListarCercanosAsync(
        double lat, double lng, int radioMetros, string? tipo = null, IEnumerable<string>? platos = null)
    {
        double degLat = radioMetros / 111_000.0;
        double degLng = radioMetros / (111_000.0 * Math.Cos(lat * Math.PI / 180.0));

        var minLat = lat - degLat;
        var maxLat = lat + degLat;
        var minLng = lng - degLng;
        var maxLng = lng + degLng;

        var q = _db.Restaurantes
            .Include(r => r.Platos)
            .Include(r => r.GustosQueSirve)
            .Include(r => r.RestriccionesQueRespeta)
            .Where(r => r.Latitud >= minLat && r.Latitud <= maxLat
                     && r.Longitud >= minLng && r.Longitud <= maxLng);

        if (!string.IsNullOrWhiteSpace(tipo) &&
            Enum.TryParse<TipoRestaurante>(tipo, true, out var tipoParsed))
        {
            q = q.Where(r => r.Tipo == tipoParsed);
        }

        if (platos is not null && platos.Any())
        {
            var set = new HashSet<PlatoComida>(
                platos.Select(p => Enum.TryParse<PlatoComida>(p, true, out var v) ? v : (PlatoComida?)null)
                      .Where(v => v.HasValue)
                      .Select(v => v!.Value));
            q = q.Where(r => r.Platos.Any(p => set.Contains(p.Plato)));
        }

        return await q
            .OrderBy(r => Math.Abs(r.Latitud - lat) + Math.Abs(r.Longitud - lng))
            .Take(200)
            .ToListAsync();
    }

    public async Task<Restaurante> ObtenerReseñasDesdeGooglePlaces(string placeId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(placeId))
            throw new ArgumentException("placeId requerido", nameof(placeId));

        var apiKey = _config["GooglePlaces:ApiKey"]
            ?? throw new InvalidOperationException("Falta GooglePlaces:ApiKey");

        // Campos a solicitar (optimizados)
        var fieldMask = string.Join(",",
            "id,displayName,primaryType,types,priceLevel,rating,userRatingCount,",
            "formattedAddress,internationalPhoneNumber,websiteUri,location,photos.name,",
            "currentOpeningHours,reviews"
        );

        // Forzar idioma español en la respuesta
        var url = $"https://places.googleapis.com/v1/places/{Uri.EscapeDataString(placeId)}?languageCode=es";

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.TryAddWithoutValidation("X-Goog-Api-Key", apiKey);
        req.Headers.TryAddWithoutValidation("X-Goog-FieldMask", fieldMask);

        var resp = await _http.SendAsync(req, ct);
        if (!resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"⚠️ Google Places API error: {resp.StatusCode}");
            return await _repo.GetByPlaceIdAsync(placeId, ct)
                ?? new Restaurante { PlaceId = placeId, Nombre = "(sin datos)" };
        }

        var details = await resp.Content.ReadFromJsonAsync<PlaceDetails>(cancellationToken: ct);
        if (details is null)
            return await _repo.GetByPlaceIdAsync(placeId, ct)
                ?? new Restaurante { PlaceId = placeId, Nombre = "(sin datos)" };

        // 🔎 Buscar restaurante existente
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
                Reviews = new List<ReseñaRestaurante>()
            };

            await _repo.AddAsync(existente, ct);
        }
        else
        {
            //  Actualizar campos básicos si ya existe
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

        // Procesar reseñas (si hay)
        if (details.Reviews is { Count: > 0 })
        {
            var reseñas = details.Reviews.Select(r => new ReseñaRestaurante
            {
                Id = Guid.NewGuid(),
                RestauranteId = existente.Id,
                Autor = r.AuthorAttribution?.DisplayName ?? "Anónimo",
                Texto = r.Text?.Text ?? "",
                Rating = r.Rating,
                Fecha = r.RelativePublishTimeDescription ?? "",
                Foto = r.AuthorAttribution?.PhotoUri
            }).ToList();

            // Evitar duplicar reseñas
            var idsExistentes = existente.Reviews?.Select(x => x.Texto)?.ToHashSet() ?? new HashSet<string>();
            var nuevas = reseñas.Where(r => !idsExistentes.Contains(r.Texto)).ToList();

            if (nuevas.Any())
            {
                await _db.ReseñasRestaurantes.AddRangeAsync(nuevas, ct);
                await _db.SaveChangesAsync(ct);
                existente.Reviews = nuevas;
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


   

}
