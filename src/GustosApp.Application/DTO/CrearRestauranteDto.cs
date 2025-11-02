using System.Text.Json;
using System.Text.Json.Serialization;

namespace GustosApp.Application.DTOs.Restaurantes
{
    public class CrearRestauranteDto
    {
        public string Nombre { get; set; } = default!;
        public string Direccion { get; set; } = default!;

        public double? Latitud { get; set; }
        public double? Longitud { get; set; }

        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lng")]
        public double? Lng { get; set; }

        public JsonElement? Horarios { get; set; }

        [JsonPropertyName("primaryType")]
        public string? PrimaryType { get; set; }

        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }

        [JsonIgnore]
        public string TypesComoJson =>
            JsonSerializer.Serialize(Types ?? new List<string>(),
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        [JsonPropertyName("gustosQueSirveIds")]
        public List<Guid>? GustosQueSirveIds { get; set; }

        [JsonPropertyName("restriccionesQueRespetaIds")]
        public List<Guid>? RestriccionesQueRespetaIds { get; set; }

        public List<string>? Platos { get; set; }

        public string? ImagenUrl { get; set; }

        public double? Valoracion { get; set; }

        [JsonIgnore]
        public (double? lat, double? lng) Coordenadas => (Lat ?? Latitud, Lng ?? Longitud);

        [JsonIgnore]
        public string? HorariosComoJson
        {
            get
            {
                if (Horarios is null) return null;
                return JsonSerializer.Serialize(Horarios, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
        }

        public string? Tipo { get; set; }
    }
}
