using System;

namespace GustosApp.Domain.Model
{
    public class RestauranteMenu
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RestauranteId { get; set; }
        public Restaurante Restaurante { get; set; } = null!;

        public string Moneda { get; set; } = "ARS";
        public string Json { get; set; } = "{}";
        public int Version { get; set; } = 1;
        public DateTime FechaActualizacionUtc { get; set; } = DateTime.UtcNow;
    }
}