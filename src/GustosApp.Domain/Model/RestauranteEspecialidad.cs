using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class RestauranteEspecialidad
    {
        public Guid Id { get; set; }
        public Guid RestauranteId { get; set; }
        public string Nombre { get; set; }

        public Restaurante Restaurante { get; set; }


        public RestauranteEspecialidad(Guid id, Guid restauranteId, string nombre)
        {
            Id = id;
            RestauranteId = restauranteId;
            Nombre = nombre;
        }
    }
}
