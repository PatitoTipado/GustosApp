using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GustosApp.Application.DTOs.Restaurantes;

using GustosApp.Application.DTO.PlacesV1;

namespace GustosApp.Application.UseCases;

public class ActualizarDetallesRestauranteUseCase
{
    private readonly IRestauranteRepository _repo;
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public ActualizarDetallesRestauranteUseCase(
        IRestauranteRepository repo, IConfiguration config, HttpClient http)
    {
        _repo = repo;
        _config = config;
        _http = http;
    }

    public async Task<RestauranteListadoDto?> HandleAsync(string placeId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(placeId))
            throw new ArgumentException("placeId requerido", nameof(placeId));

        var apiKey = _config["GooglePlaces:ApiKey"]
            ?? throw new InvalidOperationException("Falta GooglePlaces:ApiKey"); // <-- igual que Nearby

        var fieldMask =
            "id,displayName,primaryType,types,priceLevel,rating,userRatingCount," +
            "currentOpeningHours.openNow," +
            "delivery,takeout,dineIn,curbsidePickup,reservable," +
            "servesBreakfast,servesBrunch,servesLunch,servesDinner," +
            "servesBeer,servesWine,servesCocktails,servesCoffee,servesVegetarianFood," +
            "formattedAddress,location,photos.name";

        var url = $"https://places.googleapis.com/v1/places/{Uri.EscapeDataString(placeId)}";

        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.TryAddWithoutValidation("X-Goog-Api-Key", apiKey);
        req.Headers.TryAddWithoutValidation("X-Goog-FieldMask", fieldMask);

        var resp = await _http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();

        var details = await resp.Content.ReadFromJsonAsync<PlaceDetails>(cancellationToken: ct);
        if (details is null) return null;

        // upsert en tu repo
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
                HorariosJson = "{}",
                CreadoUtc = DateTime.UtcNow,
                ActualizadoUtc = DateTime.UtcNow,
                PlaceId = placeId,
                Rating = details.Rating,
                CantidadResenas = details.UserRatingCount,
                Categoria = details.PrimaryType ?? "restaurant",
                UltimaActualizacion = DateTime.UtcNow
            };
            await _repo.AddAsync(existente, ct);
        }
        else
        {
            existente.Nombre = details.DisplayName?.Text ?? existente.Nombre;
            existente.Direccion = details.FormattedAddress ?? existente.Direccion;
            existente.Latitud = details.Location?.Latitude ?? existente.Latitud;
            existente.Longitud = details.Location?.Longitude ?? existente.Longitud;
            existente.Rating = details.Rating ?? existente.Rating;
            existente.CantidadResenas = details.UserRatingCount ?? existente.CantidadResenas;
            existente.Categoria = details.PrimaryType ?? existente.Categoria;
            existente.ActualizadoUtc = DateTime.UtcNow;
            existente.UltimaActualizacion = DateTime.UtcNow;
        }

        // foto opcional (igual que Nearby)
        var fotoName = details.Photos?.FirstOrDefault()?.Name;
        if (!string.IsNullOrWhiteSpace(fotoName))
            existente.ImagenUrl = $"https://places.googleapis.com/v1/{Uri.EscapeDataString(fotoName)}/media?maxWidthPx=800&key={apiKey}";

        await _repo.SaveChangesAsync(ct);

        // map a DTO
        return new RestauranteListadoDto
        {
            Id = existente.Id,
            PlaceId = existente.PlaceId,
            Nombre = existente.Nombre,
            Direccion = existente.Direccion,
            Latitud = existente.Latitud,
            Longitud = existente.Longitud,
            ImagenUrl = existente.ImagenUrl,
            Rating = existente.Rating,
            CantidadResenas = existente.CantidadResenas,
            PrimaryType = details.PrimaryType ?? existente.Categoria,
            Types = details.Types,
            PriceLevel = details.PriceLevel,
            OpenNow = details.CurrentOpeningHours?.OpenNow,
            Delivery = details.Delivery,
            Takeout = details.Takeout,
            DineIn = details.DineIn,
            ServesVegetarianFood = details.ServesVegetarianFood,
            ServesCoffee = details.ServesCoffee,
            ServesBeer = details.ServesBeer,
            ServesWine = details.ServesWine
        };
    }
}
