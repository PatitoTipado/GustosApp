using System.Text.Json;
using System.Text.Json.Serialization;

public class ActualizarRestauranteDto
{
    public string Nombre { get; set; } = default!;
    public string Direccion { get; set; } = default!;

    public double? Latitud { get; set; }
    public double? Longitud { get; set; }

    [JsonPropertyName("lat")]
    public double? Lat { get; set; }

    [JsonPropertyName("lng")]
    public double? Lng { get; set; }

    public object? Horarios { get; set; }

    // V2

    [JsonPropertyName("primaryType")]
    public string? PrimaryType { get; set; }

    [JsonPropertyName("types")]
    public List<string>? Types { get; set; }

    public List<string>? Platos { get; set; }
    public string? ImagenUrl { get; set; }
    public decimal? Valoracion { get; set; }

    [JsonIgnore]
    public (double? lat, double? lng) Coordenadas => (Lat ?? Latitud, Lng ?? Longitud);

    [JsonIgnore]
    public string? HorariosComoJson =>
        Horarios is null ? null
        : Horarios is string s ? s
        : JsonSerializer.Serialize(Horarios, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

    public string? Tipo { get; set; }
}
