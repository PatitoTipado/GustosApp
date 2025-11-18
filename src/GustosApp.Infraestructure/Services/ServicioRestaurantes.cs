using Google.Protobuf.WellKnownTypes;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Application.Interfaces;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using GustosApp.Infraestructure.Extrerno.GooglePlacesModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
﻿using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

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



        public async Task<Restaurante> CrearAsync(string propietarioUid, CrearRestauranteDto dto)
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

        public async Task<Restaurante?> ObtenerAsync(Guid id)
        {
            var r = await _db.Restaurantes
                .Include(x => x.Platos)
                .FirstOrDefaultAsync(x => x.Id == id);
            return r is null ? null : r;
        }

        public async Task<Restaurante?> ObtenerPorPropietarioAsync(string propietarioUid)
        {
            var r = await _db.Restaurantes.AsNoTracking()
                .Include(x => x.Platos)
                .FirstOrDefaultAsync(x => x.PropietarioUid == propietarioUid);
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

        public async Task<Restaurante?> ActualizarAsync(Guid id, string solicitanteUid, bool esAdmin, ActualizarRestauranteDto dto)
        {
            var r = await _db.Restaurantes
                .Include(x => x.Platos)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (r is null) return null;

            if (!esAdmin && r.PropietarioUid != solicitanteUid)
                throw new UnauthorizedAccessException("No tenés permisos para modificar este restaurante.");

            var nombreNorm = NormalizarNombre(dto.Nombre);
            var conflictoNombre = await _db.Restaurantes
                .AnyAsync(x => x.Id != id && x.NombreNormalizado == nombreNorm);
            if (conflictoNombre)
                throw new ArgumentException("El nombre ya está en uso.");

            var primaryType = string.IsNullOrWhiteSpace(dto.PrimaryType)
                ? r.PrimaryType
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
            var coords = dto.Coordenadas;
            var latOpt = coords.lat;   // double?
            var lngOpt = coords.lng;   // double?

            if (latOpt is null || lngOpt is null)
                throw new ArgumentException("Lat/Lng requeridos para actualizar ubicación.");

            r.Nombre = dto.Nombre.Trim();
            r.NombreNormalizado = nombreNorm;
            r.Direccion = dto.Direccion.Trim();
            r.Latitud = latOpt.Value;
            r.Longitud = lngOpt.Value;
            r.HorariosJson = dto.HorariosComoJson ?? "{}";
            r.ActualizadoUtc = DateTime.UtcNow;

            r.PrimaryType = primaryType;
            r.TypesJson = JsonSerializer.Serialize(typesList);
            r.ImagenUrl = string.IsNullOrWhiteSpace(dto.ImagenUrl) ? null : dto.ImagenUrl!.Trim();
            r.Valoracion = dto.Valoracion;

            var actuales = r.Platos.Select(x => x.Plato).ToHashSet();
            var nuevos = platosParsed.ToHashSet();

            var aAgregar = nuevos.Except(actuales).ToList();
            var aQuitar = actuales.Except(nuevos).ToList();

            foreach (var p in aQuitar)
            {
                var row = r.Platos.First(x => x.Plato == p);
                _db.RestaurantePlatos.Remove(row);
            }
            foreach (var p in aAgregar)
            {
                r.Platos.Add(new RestaurantePlato { RestauranteId = r.Id, Plato = p });
            }

            await _db.SaveChangesAsync();
            await _db.Entry(r).Collection(x => x.Platos).LoadAsync();
            await _db.Entry(r).Collection(x => x.GustosQueSirve).LoadAsync();
            await _db.Entry(r).Collection(x => x.RestriccionesQueRespeta).LoadAsync();
            return (r);
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
                Console.WriteLine($" Google Places API error: {resp.StatusCode}");
                return await _repo.GetByPlaceIdAsync(placeId, ct)
                    ?? new Restaurante { PlaceId = placeId, Nombre = "(sin datos)" };
            }

            var details = await resp.Content.ReadFromJsonAsync<PlaceDetails>(cancellationToken: ct);
            if (details is null)
                return await _repo.GetByPlaceIdAsync(placeId, ct)
                    ?? new Restaurante { PlaceId = placeId, Nombre = "(sin datos)" };

            //  Buscar restaurante existente
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
                    // Reviews = new List<ReseñaRestaurante>()
                    Reviews = new List<OpinionRestaurante>() 
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
                /*var reseñas = details.Reviews.Select(r => new ReseñaRestaurante
                {
                    Id = Guid.NewGuid(),
                    RestauranteId = existente.Id,
                    Autor = r.AuthorAttribution?.DisplayName ?? "Anónimo",
                    Texto = r.Text?.Text ?? "",
                    Rating = r.Rating,
                    Fecha = r.RelativePublishTimeDescription ?? "",
                    Foto = r.AuthorAttribution?.PhotoUri
                }).ToList();*/

                var reseñas = details.Reviews.Select(r => new OpinionRestaurante(
                    usuarioId: Guid.Empty, 
                    restauranteId: existente.Id,
                    valoracion: (int)(r.Rating == 0.0 ? 1 : r.Rating),
                    titulo: r.AuthorAttribution?.DisplayName ?? "Reseña Google",
                    img: r.AuthorAttribution?.PhotoUri 
                )
                {
                 FechaCreacion = DateTime.UtcNow,
                 Autor = r.AuthorAttribution?.DisplayName ?? "Anónimo",
                 FechaTexto = r.RelativePublishTimeDescription
                }).ToList();

                // Evitar duplicar reseñas
                //var idsExistentes = existente.Reviews?.Select(x => x.Texto)?.ToHashSet() ?? new HashSet<string>();
                //var nuevas = reseñas.Where(r => !idsExistentes.Contains(r.Texto)).ToList();
                var idsExistentes = existente.Reviews?.Select(x => x.Opinion)?.ToHashSet() ?? new HashSet<string>();
                var nuevas = reseñas.Where(r => !idsExistentes.Contains(r.Opinion)).ToList();

                if (nuevas.Any())
                {
                    // await _db.ReseñasRestaurantes.AddRangeAsync(nuevas, ct);
                    await _db.OpinionesRestaurante.AddRangeAsync(nuevas, ct);
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
}
