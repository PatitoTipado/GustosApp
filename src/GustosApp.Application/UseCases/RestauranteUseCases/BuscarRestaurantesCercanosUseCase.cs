using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GustosApp.Application.DTO.PlacesV1;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Configuration;


namespace GustosApp.Application.UseCases.RestauranteUseCases
{
    public class BuscarRestaurantesCercanosUseCase
    {
        private readonly IRestauranteRepository _repo;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public BuscarRestaurantesCercanosUseCase(IRestauranteRepository repo, IConfiguration config, HttpClient http)
        {
            _repo = repo;
            _config = config;
            _http = http;
        }

        public async Task<List<Restaurante>> HandleAsync(
            double lat,
            double lng,
            int radioMetros,
            string? typesCsv = null,
            string? priceLevelsCsv = null,
            bool? openNow = null,
            double? minRating = null,
            int minUserRatings = 0,
            string? servesCsv = null,
            CancellationToken ct = default)
        {
            var cached = await _repo.GetNearbyAsync(lat, lng, radioMetros, TimeSpan.FromHours(24), ct);
            if (cached.Any() && !RequiresLiveDetails(servesCsv))
            {
                return cached
                    .Where(r => FiltroInMemory(r, typesCsv, priceLevelsCsv, openNow, minRating, minUserRatings))
                    .ToList();
            }

            var apiKey = _config["GooglePlaces:ApiKey"]
                         ?? throw new InvalidOperationException("Falta GooglePlaces:ApiKey");

            var req = BuildNearbyRequest(lat, lng, radioMetros, typesCsv);
            var nearby = await EjecutarNearbyAsync(req, apiKey, ct);

            foreach (var p in nearby.Places ?? new())
            {
                var existente = await _repo.GetByPlaceIdAsync(p.Id, ct);
                if (existente == null)
                {
                    var nuevo = new Restaurante
                    {
                        Id = Guid.NewGuid(),
                        PropietarioUid = string.Empty,
                        Nombre = p.DisplayName?.Text ?? "(sin nombre)",
                        NombreNormalizado = (p.DisplayName?.Text ?? "").Trim().ToLowerInvariant(),
                        Direccion = p.FormattedAddress ?? string.Empty,
                        Latitud = p.Location?.Latitude ?? 0,
                        Longitud = p.Location?.Longitude ?? 0,
                        PlaceId = p.Id,
                        Rating = p.Rating,
                        CantidadResenas = p.UserRatingCount,
                        Categoria = p.PrimaryType ?? "restaurant",
                        ImagenUrl = GetPhotoUrl(p.Photos?.FirstOrDefault()?.Name, apiKey),
                        UltimaActualizacion = DateTime.UtcNow
                    };

                    await _repo.AddAsync(nuevo, ct);
                }
            }

            await TrySaveIgnoringNombreNormalizadoAsync(ct);

            var final = await _repo.GetNearbyAsync(lat, lng, radioMetros, TimeSpan.Zero, ct);

            return final
                .Where(r => FiltroInMemory(r, typesCsv, priceLevelsCsv, openNow, minRating, minUserRatings))
                .ToList();
        }

        // Métodos auxiliares más limpios y reutilizables
        private static string? GetPhotoUrl(string? fotoName, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(fotoName)) return null;
            return $"https://places.googleapis.com/v1/{Uri.EscapeDataString(fotoName)}/media?maxWidthPx=800&key={apiKey}";
        }

        private static NearbyRequest BuildNearbyRequest(double lat, double lng, int radio, string? typesCsv)
        {
            var included = string.IsNullOrWhiteSpace(typesCsv)
                ? new List<string> { "restaurant" }
                : typesCsv.Split(',').Select(t => t.Trim()).ToList();

            return new NearbyRequest
            {
                IncludedTypes = included,
                MaxResultCount = 20,
                LanguageCode = "es",
                LocationRestriction = new LocationRestriction
                {
                    Circle = new Circle
                    {
                        Center = new LatLng { Latitude = lat, Longitude = lng },
                        Radius = radio
                    }
                }
            };
        }

        private async Task<NearbyResponse> EjecutarNearbyAsync(NearbyRequest req, string apiKey, CancellationToken ct)
        {
            using var httpReq = new HttpRequestMessage(HttpMethod.Post, "https://places.googleapis.com/v1/places:searchNearby");
            httpReq.Headers.TryAddWithoutValidation("X-Goog-Api-Key", apiKey);
            httpReq.Headers.TryAddWithoutValidation("X-Goog-FieldMask",
                "places.id,places.displayName,places.primaryType,places.priceLevel,places.rating,places.userRatingCount,places.location,places.photos.name,places.formattedAddress");
            httpReq.Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

            var resp = await _http.SendAsync(httpReq, ct);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<NearbyResponse>(cancellationToken: ct) ?? new NearbyResponse();
        }

        private static bool FiltroInMemory(Restaurante r, string? typesCsv, string? priceLevelsCsv,
            bool? openNow, double? minRating, int minUserRatings)
        {
            if (minRating.HasValue && (r.Rating ?? 0) < minRating.Value)
                return false;

            if (minUserRatings > 0 && (r.CantidadResenas ?? 0) < minUserRatings)
                return false;

            return true;
        }

        private async Task<bool> TrySaveIgnoringNombreNormalizadoAsync(CancellationToken ct)
        {
            try
            {
                await _repo.SaveChangesAsync(ct);
                return true;
            }
            catch (Exception ex) when (IsNombreNormalizadoUniqueViolation(ex))
            {
                return false;
            }
        }

        private static bool IsNombreNormalizadoUniqueViolation(Exception ex)
        {
            var msg = ex.InnerException?.Message ?? ex.Message;
            return msg.Contains("UX_Restaurantes_NombreNormalizado", StringComparison.OrdinalIgnoreCase)
                   || msg.Contains("Cannot insert duplicate key", StringComparison.OrdinalIgnoreCase)
                   || msg.Contains("2601");
        }

        private static bool RequiresLiveDetails(string? servesCsv) =>
            !string.IsNullOrWhiteSpace(servesCsv);
    }

}

