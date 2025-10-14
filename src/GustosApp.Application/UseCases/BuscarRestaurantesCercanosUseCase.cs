using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GustosApp.Application.DTO.PlacesV1;
using GustosApp.Application.DTOs.Restaurantes;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Configuration;

namespace GustosApp.Application.UseCases
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

        public async Task<List<RestauranteListadoDto>> HandleAsync(
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
            // cache-first
            var cached = await _repo.GetNearbyAsync(lat, lng, radioMetros, maxAge: TimeSpan.FromHours(24), ct);
            var cachedMapped = cached.Select(MapToListado).ToList();


            if (cachedMapped.Count > 0 && !RequiresLiveDetails(servesCsv, openNow, priceLevelsCsv))
            {
                var filteredCached = ApplyInMemoryFilters(cachedMapped, typesCsv, priceLevelsCsv, openNow, minRating, minUserRatings, servesCsv);
                if (filteredCached.Count > 0)
                    return filteredCached;
            }

            //Nearby Search v1 (restaurants only)
            var apiKey = _config["GooglePlaces:ApiKey"] ?? throw new InvalidOperationException("Falta GooglePlaces:ApiKey");
            var fieldMask = "places.id,places.displayName,places.primaryType,places.types,places.priceLevel,places.rating,places.userRatingCount,places.currentOpeningHours.openNow,places.location,places.photos.name,places.formattedAddress";

            var req = new NearbyRequest
            {
                IncludedTypes = ParseCsv(typesCsv) ?? new List<string> { "restaurant" },
                MaxResultCount = 20,
                LanguageCode = "es",
                LocationRestriction = new LocationRestriction
                {
                    Circle = new Circle
                    {
                        Center = new LatLng { Latitude = lat, Longitude = lng },
                        Radius = radioMetros
                    }
                }
            };

            var httpReq = new HttpRequestMessage(HttpMethod.Post, "https://places.googleapis.com/v1/places:searchNearby");
            httpReq.Headers.TryAddWithoutValidation("X-Goog-Api-Key", apiKey);
            httpReq.Headers.TryAddWithoutValidation("X-Goog-FieldMask", fieldMask);
            httpReq.Content = new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json");

            var httpResp = await _http.SendAsync(httpReq, ct);
            httpResp.EnsureSuccessStatusCode();
            var nearby = await httpResp.Content.ReadFromJsonAsync<NearbyResponse>(cancellationToken: ct);

            var items = new List<RestauranteListadoDto>();
            foreach (var p in nearby?.Places ?? new())
            {
                var placeId = p.Id;
                var fotoName = p.Photos?.FirstOrDefault()?.Name;
                string? photoUrl = null;
                if (!string.IsNullOrWhiteSpace(fotoName))
                {

                    photoUrl = $"https://places.googleapis.com/v1/{Uri.EscapeDataString(fotoName)}/media?maxWidthPx=800&key={apiKey}";
                }


                var existente = await _repo.GetByPlaceIdAsync(placeId, ct);
                Guid idParaDto;

                if (existente is null)
                {
                    var nuevo = new Restaurante
                    {
                        Id = Guid.NewGuid(),
                        PropietarioUid = string.Empty,
                        Nombre = p.DisplayName?.Text ?? "(sin nombre)",
                        NombreNormalizado = (p.DisplayName?.Text ?? string.Empty).Trim().ToLowerInvariant(),
                        Direccion = p.FormattedAddress ?? string.Empty,   // <-- cambio aquí
                        Latitud = p.Location?.Latitude ?? 0,
                        Longitud = p.Location?.Longitude ?? 0,
                        HorariosJson = "{}",
                        CreadoUtc = DateTime.UtcNow,
                        ActualizadoUtc = DateTime.UtcNow,
                        PlaceId = placeId,
                        Rating = p.Rating,
                        CantidadResenas = p.UserRatingCount,
                        Categoria = p.PrimaryType ?? "restaurant",
                        ImagenUrl = photoUrl,
                        UltimaActualizacion = DateTime.UtcNow
                    };
                    await _repo.AddAsync(nuevo, ct);
                    idParaDto = nuevo.Id;                // <-- usa el Id real insertado
                }
                else
                {
                    idParaDto = existente.Id;
                }

                items.Add(new RestauranteListadoDto
                {
                    Id = idParaDto,
                    PlaceId = placeId,
                    Nombre = p.DisplayName?.Text ?? "",
                    Direccion = p.FormattedAddress ?? string.Empty,       // <-- cambio aquí
                    Latitud = p.Location?.Latitude ?? 0,
                    Longitud = p.Location?.Longitude ?? 0,
                    ImagenUrl = photoUrl,
                    Rating = p.Rating,
                    CantidadResenas = p.UserRatingCount,
                    PrimaryType = p.PrimaryType,
                    Types = p.Types,
                    PriceLevel = p.PriceLevel,
                    OpenNow = p.CurrentOpeningHours?.OpenNow
                });

            }


            await _repo.SaveChangesAsync(ct);


            if (RequiresLiveDetails(servesCsv, openNow: null, priceLevelsCsv: null))
            {
                items = await EnrichWithDetailsAndFilterAsync(items, servesCsv, minRating, minUserRatings, apiKey, ct);
            }
            else
            {

                items = ApplyInMemoryFilters(items, typesCsv, priceLevelsCsv, openNow, minRating, minUserRatings, servesCsv);
            }

            return items;
        }

        private static List<string>? ParseCsv(string? csv) =>
            string.IsNullOrWhiteSpace(csv) ? null :
            csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
               .Select(s => s).ToList();

        private static bool RequiresLiveDetails(string? servesCsv, bool? openNow, string? priceLevelsCsv)
        {

            return !string.IsNullOrWhiteSpace(servesCsv);
        }

        private async Task<List<RestauranteListadoDto>> EnrichWithDetailsAndFilterAsync(
            List<RestauranteListadoDto> items,
            string? servesCsv,
            double? minRating,
            int minUserRatings,
            string apiKey,
            CancellationToken ct)
        {
            var serveFlags = ParseCsv(servesCsv)?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new();

            var list = new List<RestauranteListadoDto>();
            foreach (var it in items)
            {
                var details = await GetPlaceDetailsAsync(it, apiKey, ct);
                if (details is null) continue;

                ApplyDetailsToDto(it, details);

                if (minRating.HasValue && (it.Rating ?? 0) < minRating.Value) continue;
                if (minUserRatings > 0 && (it.CantidadResenas ?? 0) < minUserRatings) continue;

                if (serveFlags.Count > 0)
                {
                    bool ok = true;
                    foreach (var f in serveFlags)
                    {
                        ok &= f switch
                        {
                            "servesVegetarianFood" => it.ServesVegetarianFood == true,
                            "servesCoffee" => it.ServesCoffee == true,
                            "servesBeer" => it.ServesBeer == true,
                            "servesWine" => it.ServesWine == true,
                            "delivery" => it.Delivery == true,
                            "takeout" => it.Takeout == true,
                            "dineIn" => it.DineIn == true,
                            _ => true
                        };
                        if (!ok) break;
                    }
                    if (!ok) continue;
                }

                list.Add(it);
            }
            return list;
        }

        private async Task<PlaceDetails?> GetPlaceDetailsAsync(
    RestauranteListadoDto it,
    string apiKey,
    CancellationToken ct)
        {

            if (string.IsNullOrWhiteSpace(it.PlaceId))
                return null;

            var fieldMask =
                "id,displayName,primaryType,types,priceLevel,rating,userRatingCount," +
                "currentOpeningHours.openNow," +
                "delivery,takeout,dineIn,curbsidePickup,reservable," +
                "servesBreakfast,servesBrunch,servesLunch,servesDinner," +
                "servesBeer,servesWine,servesCocktails,servesCoffee,servesVegetarianFood";

            var url = $"https://places.googleapis.com/v1/places/{Uri.EscapeDataString(it.PlaceId)}";

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.TryAddWithoutValidation("X-Goog-Api-Key", apiKey);
            req.Headers.TryAddWithoutValidation("X-Goog-FieldMask", fieldMask);

            var resp = await _http.SendAsync(req, ct);
            if (!resp.IsSuccessStatusCode)
                return null;

            // Si tu tipo PlaceDetails está en DTO/PlacesV1, usa ese
            var details = await resp.Content.ReadFromJsonAsync<PlaceDetails>(cancellationToken: ct);
            return details;
        }


        private static void ApplyDetailsToDto(RestauranteListadoDto it, PlaceDetails d)
        {
            it.PriceLevel = d.PriceLevel ?? it.PriceLevel;
            it.Rating = d.Rating ?? it.Rating;
            it.CantidadResenas = d.UserRatingCount ?? it.CantidadResenas;
            it.OpenNow = d.CurrentOpeningHours?.OpenNow ?? it.OpenNow;
            it.PrimaryType = d.PrimaryType ?? it.PrimaryType;
            it.Types = d.Types ?? it.Types;
            it.Delivery = d.Delivery;
            it.Takeout = d.Takeout;
            it.DineIn = d.DineIn;
            it.ServesVegetarianFood = d.ServesVegetarianFood;
            it.ServesCoffee = d.ServesCoffee;
            it.ServesBeer = d.ServesBeer;
            it.ServesWine = d.ServesWine;
        }

        private static RestauranteListadoDto MapToListado(Restaurante e) => new RestauranteListadoDto
        {
            Id = e.Id,
            PlaceId = e.PlaceId,
            Nombre = e.Nombre,
            Direccion = e.Direccion,
            Latitud = e.Latitud,
            Longitud = e.Longitud,
            ImagenUrl = e.ImagenUrl,
            Rating = e.Rating,
            CantidadResenas = e.CantidadResenas,
            PrimaryType = e.Categoria,
            Types = null,
            PriceLevel = null,
            OpenNow = null
        };

        private static List<RestauranteListadoDto> ApplyInMemoryFilters(
            List<RestauranteListadoDto> items,
            string? typesCsv,
            string? priceLevelsCsv,
            bool? openNow,
            double? minRating,
            int minUserRatings,
            string? servesCsv)
        {
            var filtered = items.AsEnumerable();

            var types = ParseCsv(typesCsv)?.ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (types is { Count: > 0 })
            {
                filtered = filtered.Where(i =>
                    (i.PrimaryType != null && types.Contains(i.PrimaryType)) ||
                    (i.Types != null && i.Types.Any(t => types.Contains(t)))
                );
            }

            var priceLevels = ParseCsv(priceLevelsCsv)?.ToHashSet(StringComparer.OrdinalIgnoreCase);
            if (priceLevels is { Count: > 0 })
            {
                filtered = filtered.Where(i => i.PriceLevel != null && priceLevels.Contains(i.PriceLevel));
            }

            if (openNow.HasValue)
            {
                filtered = filtered.Where(i => i.OpenNow == openNow.Value);
            }

            if (minRating.HasValue)
            {
                filtered = filtered.Where(i => (i.Rating ?? 0) >= minRating.Value);
            }
            if (minUserRatings > 0)
            {
                filtered = filtered.Where(i => (i.CantidadResenas ?? 0) >= minUserRatings);
            }


            return filtered.ToList();
        }
    }
}

