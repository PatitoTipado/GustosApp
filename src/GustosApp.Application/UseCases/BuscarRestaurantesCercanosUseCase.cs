using System;
using System.Collections.Generic;
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
    public class BuscarRestaurantesCercanosUseCase
    {
        private readonly IRestauranteRepository _repo;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public BuscarRestaurantesCercanosUseCase(
            IRestauranteRepository repo,
            IConfiguration config,
            HttpClient http)
        {
            _repo = repo;
            _config = config;
            _http = http;
        }

        public async Task<List<Restaurante>> HandleAsync(double lat, double lng, int radio, CancellationToken ct)
        {
            string apiKey = _config["GoogleApiKey"];
            string url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?" +
                         $"location={lat},{lng}&radius={radio}&type=restaurant&key={apiKey}";

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

            if (nuevos.Any())
                await _repo.SaveChangesAsync(ct);

            return nuevos;
        }
    }
}
