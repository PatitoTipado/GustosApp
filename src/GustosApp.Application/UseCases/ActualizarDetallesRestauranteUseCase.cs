using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

public class ActualizarDetallesRestauranteUseCase
{
    private readonly IRestauranteRepository _repo;
    private readonly IReviewRepository _reseñas;
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public ActualizarDetallesRestauranteUseCase(
        IRestauranteRepository repo,
        IReviewRepository reseñas,
        HttpClient http,
        IConfiguration config)
    {
        _repo = repo;
        _reseñas = reseñas;
        _http = http;
        _config = config;
    }

    public async Task<Restaurante> HandleAsync(string placeId, CancellationToken ct)
    {
        string? apiKey = _config["GoogleMaps:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("Google API key is not configured.");

        var restaurante = await _repo.GetByPlaceIdAsync(placeId, ct)
                          ?? throw new Exception("Restaurante no encontrado en la base de datos.");

        // 🚀 Cargar detalles desde Google
        var url = $"https://maps.googleapis.com/maps/api/place/details/json?" +
                  $"place_id={placeId}&fields=name,formatted_address,types,rating,user_ratings_total,website,review,photos&key={apiKey}";

        var response = await _http.GetFromJsonAsync<GooglePlaceDetailsResponse>(url, ct)
                       ?? throw new Exception("No se pudo obtener respuesta de Google Places.");

        var data = response.Result ?? throw new Exception($"No hay resultado válido para {placeId}");

        // 🧩 Actualizar campos básicos
        restaurante.Nombre = data.Name ?? restaurante.Nombre;
        restaurante.Direccion = data.FormattedAddress ?? restaurante.Direccion;
        restaurante.Rating = data.Rating ?? restaurante.Rating;
        restaurante.CantidadResenas = data.UserRatingsTotal ?? restaurante.CantidadResenas;
        restaurante.Categoria = string.Join(",", data.Types ?? []);
        restaurante.UltimaActualizacion = DateTime.UtcNow;

        // 🧹 Eliminar reseñas viejas correctamente
        await _reseñas.RemoveByRestauranteIdAsync(restaurante.Id, ct);

        // ✍️ Agregar reseñas nuevas
        if (data.Reviews is not null && data.Reviews.Count > 0)
        {
            foreach (var review in data.Reviews)
            {
                var nueva = new ReviewRestaurante
                {
                    Autor = review.AuthorName ?? "Desconocido",
                    Rating = review.Rating,
                    Texto = review.Text ?? "",
                    RestauranteId = restaurante.Id
                };
                await _reseñas.AddAsync(nueva, ct);
            }
        }

       
        await _repo.SaveChangesAsync(ct);

        return restaurante;
    }
}
