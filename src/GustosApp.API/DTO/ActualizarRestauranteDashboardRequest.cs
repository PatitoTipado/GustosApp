using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GustosApp.API.DTO
{
    public class ActualizarRestauranteDashboardRequest
    {
        
        [JsonPropertyName("direccion")]
        public string? Direccion { get; set; }

        [JsonPropertyName("lat")]
        public double? Latitud { get; set; }

        [JsonPropertyName("lng")]
        public double? Longitud { get; set; }

        [JsonPropertyName("horariosJson")]
        public string? HorariosJson { get; set; }

        [JsonPropertyName("webUrl")]
        public string? WebUrl { get; set; }

        [JsonPropertyName("gustosQueSirveIds")]
        public List<Guid>? GustosQueSirveIds { get; set; }

        [JsonPropertyName("restriccionesQueRespetaIds")]
        public List<Guid>? RestriccionesQueRespetaIds { get; set; }
    }
}
