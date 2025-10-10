using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class Restaurante
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string PlaceId { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public double? Rating { get; set; }
        public int? CantidadResenas { get; set; }
        public string? Categoria { get; set; }
        public string? ImagenUrl { get; set; }
        public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;
    }
}
