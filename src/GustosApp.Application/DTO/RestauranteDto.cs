using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    public class RestauranteDto
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double Rating { get; set; }
        public string GooglePlaceId { get; set; }
    }

}
