using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.API.DTO;
using GustosApp.Application.DTO;
using GustosApp.Domain.Model;

namespace GustosApp.API.DTO
{
    public class RestauranteDto
    {
        public Guid Id { get; set; }
        public string PropietarioUid { get; set; } = string.Empty;

        public object? Horarios { get; set; }
        public DateTime CreadoUtc { get; set; }
        public DateTime ActualizadoUtc { get; set; }

        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
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
        public string? Tipo { get; internal set; }



        // --- CONSTRUCTOR 1: Vacío (Requerido por serializadores/mapeadores) ---
        public RestauranteDto()
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
        public RestauranteDto(
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
            Lat = latitud;
            Lng = longitud;
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
}

