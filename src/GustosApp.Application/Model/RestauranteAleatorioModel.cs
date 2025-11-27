using System;
using System.Collections.Generic;

namespace GustosApp.Application.Model
{
    public class RestauranteAleatorioModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double? Rating { get; set; }
        public int? CantidadResenas { get; set; }
        public string? Categoria { get; set; }
        public string? ImagenUrl { get; set; }
        public decimal? Valoracion { get; set; }
        public string? WebUrl { get; set; }
        public string PlaceId { get; set; } = string.Empty;
        public List<string> Gustos { get; set; } = new List<string>();
        public List<string> Restricciones { get; set; } = new List<string>();
    }

    public class ObtenerRestaurantesAleatoriosRequestModel
    {
        public int Cantidad { get; set; } = 1;
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public int? RadioMetros { get; set; }
    }
}
