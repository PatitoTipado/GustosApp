using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;
using GustosApp.Domain.Interfaces;
using GustosApp.Domain.Model;
using Microsoft.Extensions.Configuration;

namespace GustosApp.Application.UseCases
{
    public class BuscarRestaurantesCercanosUseCase(IRestauranteRepository repo, IConfiguration config, HttpClient http)
    {
        private readonly IRestauranteRepository _repo = repo;
        private readonly IConfiguration _config = config;
        private readonly HttpClient _http = http;

        public async Task<List<Restaurante>> HandleAsync(double lat, double lng, int radio, CancellationToken ct)
        {
            string? apiKey = _config["GoogleMaps:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Google API key is not configured.");

            string url =
         $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
         $"location={lat.ToString(CultureInfo.InvariantCulture)}," +
         $"{lng.ToString(CultureInfo.InvariantCulture)}" +
         $"&radius={radio}" +
         $"&type=restaurant" +
         $"&language=es" +   
         $"&key={apiKey}";


            var json = await _http.GetStringAsync(url, ct);
            
            var response = await _http.GetFromJsonAsync<GooglePlacesResponse>(url, ct)
                           ?? throw new Exception("No se pudo obtener respuesta de Google Places.");

            var nuevos = new List<Restaurante>();

            foreach (var r in response.Results)
            {
                var existente = await _repo.GetByPlaceIdAsync(r.PlaceId, ct);
                if (existente != null) continue; // Ya existe  

                var nuevo = new Restaurante
                {
                    PlaceId = r.PlaceId,
                    Nombre = r.Name,
                    NombreNormalizado = r.Name.Trim().ToLowerInvariant(),
                    Direccion = r.Vicinity ?? "",
                    Latitud = r.Geometry.Location.Lat,
                    Longitud = r.Geometry.Location.Lng,
                    Rating = r.Rating,
                    CantidadResenas = r.UserRatingsTotal,
                    Categoria = string.Join(",", r.Types),
                    ImagenUrl = r.Photos?.FirstOrDefault()?.PhotoReference != null
                        ? $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photo_reference={r.Photos.First().PhotoReference}&key={apiKey}"
                        : null
                };

                nuevos.Add(nuevo);
                await _repo.AddAsync(nuevo, ct);
            }

            if (nuevos.Count > 0)
                await _repo.SaveChangesAsync(ct);

            return nuevos;
        }
    }
}
