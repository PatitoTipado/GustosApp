using System;
using GustosApp.Domain.Model.@enum;

namespace GustosApp.Domain.Model
{

    public class RestauranteImagen
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RestauranteId { get; set; }
        public Restaurante Restaurante { get; set; } = null!;

        public TipoImagenRestaurante Tipo { get; set; }
        public string Url { get; set; } = string.Empty;
        public int? Orden { get; set; }
        public DateTime FechaCreacionUtc { get; set; } = DateTime.UtcNow;
    }
}