using System;

namespace GustosApp.Domain.Model
{
    public class RestaurantePlato
    {
        public Guid RestauranteId { get; set; }
        public Restaurante Restaurante { get; set; } = null!;

        public PlatoComida Plato { get; set; }
    }
}
