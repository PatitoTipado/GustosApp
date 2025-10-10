
using System;

namespace GustosApp.Application.DTOs.Restaurantes
{
    public class RestauranteDto
    {
        public Guid Id { get; set; }
        public string PropietarioUid { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public object? Horarios { get; set; }
        public DateTime CreadoUtc { get; set; }
        public DateTime ActualizadoUtc { get; set; }
    }
}
