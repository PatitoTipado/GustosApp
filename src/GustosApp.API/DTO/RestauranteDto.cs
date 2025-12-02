using GustosApp.API.DTO;
using GustosApp.API.DTO;
using GustosApp.Domain.Model;
using GustosApp.Domain.Model.@enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace GustosApp.API.DTO
{


    public class RestauranteDTO
    {
       


        public Guid Id { get; set; }
        public string PropietarioUid { get; set; } = string.Empty;

        public object? Horarios { get; set; }
        public DateTime CreadoUtc { get; set; }
        public DateTime ActualizadoUtc { get; set; }
        public bool EsDeLaApp { get; set; }
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


    public class ImagenesResponse
    {
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public List<RestauranteImagenDto> Items { get; set; } = new();
    }
    public class CrearRestauranteDto
    {
        public string? Nombre { get; set; } = default!;
        public string Direccion { get; set; } = default!;

        public string ?WebsiteUrl { get; set; } = default!;


        [JsonPropertyName("lat")]
        public string? Lat { get; set; } // Cambiar a string

        [JsonPropertyName("lng")]
        public string? Lng { get; set; } // Cambiar a string

        // Propiedades helper para obtener como double
        public double? LatAsDouble => !string.IsNullOrWhiteSpace(Lat)
            ? double.Parse(Lat.Replace(",", "."), CultureInfo.InvariantCulture)
            : null;

        public double? LngAsDouble => !string.IsNullOrWhiteSpace(Lng)
            ? double.Parse(Lng.Replace(",", "."), CultureInfo.InvariantCulture)
            : null;

        [FromForm(Name = "horariosJson")]
        public string? HorariosJson { get; set; }
        
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
    public class DatosSolicitudRestauranteDto
    {
        public List<ItemSimpleDto> Gustos { get; set; }
        public List<ItemSimpleDto> Restricciones{ get; set; }
    }



    public class RestauranteDetalleDto
    {
        public Guid Id { get; set; }

        public bool EsDeLaApp { get; set; }
        public string Nombre { get; set; } = "";
        public string Direccion { get; set; } = "";
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public string? WebUrl { get; set; }

        public double? Rating { get; set; }

        public double RatingCalculado { get; set; }
        public int? CantidadResenas { get; set; }
        public string? Categoria { get; set; }
        public string PrimaryType { get; set; } = "restaurant";

        // ==================
        // IMÁGENES
        // ==================
        public string? LogoUrl { get; set; }          // Tipo 4
        public string? ImagenDestacada { get; set; }  // Tipo 0
        public List<string> ImagenesInterior { get; set; } = new();
        public List<string> ImagenesComida { get; set; } = new();

        // ==================
        // HORARIOS
        // ==================
        public string HorariosJson { get; set; } = "{}";

        // ==================
        // MENÚ OCR PARSEADO
        // ==================
        public RestauranteMenuDto? Menu { get; set; }

        // ==================
        // REVIEWS
        // ==================
        public List<OpinionRestauranteDto> ReviewsLocales { get; set; } = new();
        public List<OpinionRestauranteDto> ReviewsGoogle { get; set; } = new();
        public ICollection<GustoDto> GustosQueSirve { get; set; }
        public ICollection<RestriccionResponse> RestriccionesQueRespeta { get; set; }

        public bool esFavorito { get; set; }

    }
    public class RestauranteMenuDto
    {
        [JsonPropertyName("nombreMenu")]
        public string NombreMenu { get; set; } = "";

        [JsonPropertyName("moneda")]
        public string Moneda { get; set; } = "ARS";

        [JsonPropertyName("categorias")]
        public List<CategoriaMenuDto> Categorias { get; set; } = new();
    }

    public class CategoriaMenuDto
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = "";

        [JsonPropertyName("items")]
        public List<ItemMenuDto> Items { get; set; } = new();
    }

    public class ItemMenuDto
    {
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = "";

        [JsonPropertyName("descripcion")]
        public string? Descripcion { get; set; }

        [JsonPropertyName("precios")]
        public List<PrecioMenuDto> Precios { get; set; } = new();
    }

    public class PrecioMenuDto
    {
        [JsonPropertyName("tamaño")]
        public string Tamaño { get; set; } = "";

        [JsonPropertyName("monto")]
        public decimal Monto { get; set; }
    }

    public class RestauranteResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public double? Rating { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
    }
    public class RestauranteMetricasDashboardResponse
    {
        public Guid RestauranteId { get; set; }

        public int TotalTop3Individual { get; set; }

        public int TotalTop3Grupo { get; set; }

        public int TotalVisitasPerfil { get; set; }

        //public List<UsuarioRestauranteFavoritoDTO> RestauranteFavoritos { get; set; }

        public List<FavoritosPorDiaDto> FavoritosPorDia { get;set;}

        public static List<FavoritosPorDiaDto> CountFavoritosPorDiaAsync(List<UsuarioRestauranteFavoritoDTO> lista)
        {
            var query = lista
                .GroupBy(x => x.FechaAgregado.Date)
                .Select(g => new FavoritosPorDiaDto
                {
                    Fecha = g.Key,
                    Cantidad = g.Count()
                })
                .OrderBy(r => r.Fecha)
                .ToList();

            return query;
        }


        public static List<UsuarioRestauranteFavoritoDTO> convertidorDeFavoritos(List<UsuarioRestauranteFavorito> entity)
        {
            List<UsuarioRestauranteFavoritoDTO> lista = new List<UsuarioRestauranteFavoritoDTO>();

            if (entity!=null)
            {
                foreach(var fav in entity)
                {
                    lista.Add(FromEntity(fav));
                }
            }

            return lista;
        }

        public static UsuarioRestauranteFavoritoDTO FromEntity(UsuarioRestauranteFavorito entity)
        {
            if (entity == null) return null;

            return new UsuarioRestauranteFavoritoDTO
            {
                Id = entity.Id,
                UsuarioId = entity.UsuarioId,
                RestauranteId = entity.RestauranteId,
                FechaAgregado = entity.FechaAgregado,
                UsuarioNombre = entity.Usuario?.Nombre,          // suponiendo que Usuario tiene Nombre
                RestauranteNombre = entity.Restaurante?.Nombre   // suponiendo que Restaurante tiene Nombre
            };
        }


    }

    public class UsuarioRestauranteFavoritoDTO
    {
        public int Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid RestauranteId { get; set; }
        public DateTime FechaAgregado { get; set; }
        public string UsuarioNombre { get; set; }
        public string RestauranteNombre { get; set; }
    }
    public class FavoritosPorDiaDto
    {
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
    }

    public class BuscarRestaurantesRequest
    {
        public string Texto { get; set; } = string.Empty;
    }

    public class RecomendacionResponse
    {
        public Guid RestauranteId { get; set; }
        public string Explicacion { get; set; }
    }

    public class RestauranteFavoritoDto
    {
        public Guid RestauranteId { get; set; }
        public string Nombre { get; set; } = "";
        public string LogoUrl { get; set; } = "";
        public double Rating { get; set; }
        public bool EsFavorito { get; set; } = true; 
    }
}




