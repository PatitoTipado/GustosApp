using System;

namespace GustosApp.Domain.Model
{
    
    /// registro de restaurantes visitados por un usuario.
    /// es para restaurantes creados en la app (RestauranteId) o de places (PlaceId).
    /// se elige como identificador expuesto el PlaceId si existe, de lo contrario el Guid del restaurante.
    
    public class UsuarioRestauranteVisitado
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public Guid? RestauranteId { get; set; }
        public Restaurante? Restaurante { get; set; }

        public string? PlaceId { get; set; } // v1 Places, puede ser null si es un restaurante creado por usuarios

        public string Nombre { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        public DateTime FechaVisitaUtc { get; set; } = DateTime.UtcNow;
    }
}
