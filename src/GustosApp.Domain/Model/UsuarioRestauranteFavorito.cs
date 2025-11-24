using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class UsuarioRestauranteFavorito
    {
        public int Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid RestauranteId { get; set; }
        public DateTime FechaAgregado { get; set; }

        public Usuario Usuario { get; set; }
        public Restaurante Restaurante { get; set; }


    }
}
