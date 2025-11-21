using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;


namespace GustosApp.API.DTO
{


    public class RestauranteDTO
    {
        public Guid Id { get; set; }
        public string PropietarioUid { get; set; } = string.Empty;

        public object? Horarios { get; set; }
        public DateTime CreadoUtc { get; set; }
        public DateTime ActualizadoUtc { get; set; }

        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double Rating { get; set; }
        public string GooglePlaceId { get; set; }

        public string? PrimaryType { get; set; }
        public IReadOnlyList<string> Types { get; set; } = Array.Empty<string>();
        public string? ImagenUrl { get; set; }
        public decimal? Valoracion { get; set; }
        public List<string> Platos { get; set; }
        public ICollection<GustoDto> GustosQueSirve { get; set; }
        public ICollection<RestriccionResponse> RestriccionesQueRespeta { get; set; }
        public double Score { get; set; }
        public string? Tipo { get; set; }




        // --- CONSTRUCTOR 1: Vacío (Requerido por serializadores/mapeadores) ---
        public RestauranteDTO()
        {
            // Inicializar colecciones para evitar NullReferenceException
            PropietarioUid = string.Empty;
            Nombre = string.Empty;
            Direccion = string.Empty;
            PrimaryType = null;
            Types = Array.Empty<string>();
            Platos = new List<string>();
            GustosQueSirve = new List<GustoDto>();
            RestriccionesQueRespeta = new List<RestriccionResponse>();
        }

        // --- CONSTRUCTOR 2: Completo (Usado para el mapeo con Score) ---
        public RestauranteDTO(
            Guid id, string propietarioUid, string nombre, string direccion, double latitud,
            double longitud, object? horarios, DateTime creadoUtc, DateTime actualizadoUtc,
            string? primaryType, IReadOnlyList<string>? types, string? imagenUrl, decimal? valoracion, List<string> platos,
            ICollection<GustoDto> gustosQueSirve, ICollection<RestriccionResponse> restriccionesQueRespeta,
            double score)
        {
            Id = id;
            PropietarioUid = propietarioUid ?? string.Empty;
            Nombre = nombre ?? string.Empty;
            Direccion = direccion ?? string.Empty;
            Latitud = latitud;
            Longitud = longitud;
            Horarios = horarios;
            CreadoUtc = creadoUtc;
            ActualizadoUtc = actualizadoUtc;
            PrimaryType = primaryType;
            Types = types ?? Array.Empty<string>();
            ImagenUrl = imagenUrl;
            Valoracion = valoracion;
            Platos = platos ?? new List<string>();
            GustosQueSirve = gustosQueSirve ?? new List<GustoDto>();
            RestriccionesQueRespeta = restriccionesQueRespeta ?? new List<RestriccionResponse>();
            Score = score;
        }

    }
                public class RestauranteImagenDto
        {
            public Guid Id { get; set; }
            public string Tipo { get; set; } = string.Empty; // perfil|principal|interior|comida|menu
            public string Url { get; set; } = string.Empty;
            public int? Orden { get; set; }
            public DateTime FechaCreacionUtc { get; set; }
            public string? MiniaturaUrl { get; set; }
        }

        public class RestauranteListadoDto
        {
            public Guid Id { get; set; }
            public string PlaceId { get; set; } = string.Empty; // Google Places ID
            public string Nombre { get; set; } = string.Empty;
            public string? Direccion { get; set; }
            public double Latitud { get; set; }
            public double Longitud { get; set; }
            public string? ImagenUrl { get; set; }

            public double? Rating { get; set; }
            public int? CantidadResenas { get; set; }

            // v1 New fields
            public string? PrimaryType { get; set; }
            public List<string>? Types { get; set; }
            public string? PriceLevel { get; set; }
            public bool? OpenNow { get; set; }

            // serves (subset)
            public bool? Delivery { get; set; }
            public bool? Takeout { get; set; }
            public bool? DineIn { get; set; }
            public bool? ServesVegetarianFood { get; set; }
            public bool? ServesCoffee { get; set; }
            public bool? ServesBeer { get; set; }
            public bool? ServesWine { get; set; }
        }


        public class RestauranteMenuDto
        {
            public Guid Id { get; set; }
            public Guid RestauranteId { get; set; }
            public string Moneda { get; set; } = "ARS";
            public int Version { get; set; } = 1;
            public DateTime FechaActualizacionUtc { get; set; }

            public JsonElement? Menu { get; set; }
            public string? MenuRaw { get; set; }
        }

    public class ImagenesResponse
    {
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public List<RestauranteImagenDto> Items { get; set; } = new();
    }
    public class CrearRestauranteDto
    {
        public string Nombre { get; set; } = default!;
        public string Direccion { get; set; } = default!;

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
        public string TypesJson =>
            Types is null ? "[]" :
            JsonSerializer.Serialize(
                Types.Select(t => t.Trim().ToLowerInvariant()),
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

        [JsonIgnore]
        public string? HorariosJson =>
            Horarios?.GetRawText(); //  ORO. No hace doble serialización.

        [JsonPropertyName("gustosQueSirveIds")]
        public List<Guid>? GustosQueSirveIds { get; set; }

        [JsonPropertyName("restriccionesQueRespetaIds")]
        public List<Guid>? RestriccionesQueRespetaIds { get; set; }

        // Imágenes
        public IFormFile? ImagenDestacada { get; set; }
        public List<IFormFile>? ImagenesInterior { get; set; }
        public List<IFormFile>? ImagenesComidas { get; set; }
        public IFormFile? ImagenMenu { get; set; }
        public IFormFile? Logo { get; set; }
    }


}




