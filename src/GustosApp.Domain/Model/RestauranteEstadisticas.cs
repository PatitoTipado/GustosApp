using System;

namespace GustosApp.Domain.Model
{

    public class RestauranteEstadisticas
    {
        public Guid RestauranteId { get; set; }
        public Restaurante Restaurante { get; set; } = null!;

        public int TotalTop3Individual { get; set; }
        public int TotalTop3Grupo { get; set; }
        public int TotalVisitasPerfil { get; set; }

        public DateTime FechaCreacionUtc { get; set; } = DateTime.UtcNow;
        public DateTime UltimaActualizacionUtc { get; set; } = DateTime.UtcNow;

        public void IncrementarTop3Individual(int cantidad = 1)
        {
            if (cantidad <= 0) return;
            TotalTop3Individual += cantidad;
            UltimaActualizacionUtc = DateTime.UtcNow;
        }

        public void IncrementarTop3Grupo(int cantidad = 1)
        {
            if (cantidad <= 0) return;
            TotalTop3Grupo += cantidad;
            UltimaActualizacionUtc = DateTime.UtcNow;
        }

        public void IncrementarVisitaPerfil(int cantidad = 1)
        {
            if (cantidad <= 0) return;
            TotalVisitasPerfil += cantidad;
            UltimaActualizacionUtc = DateTime.UtcNow;
        }
    }
}
