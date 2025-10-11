using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    public class RestauranteDto
    {

        public Guid Id { get; set; }
        public string PropietarioUid { get; set; } = string.Empty;

        public object? Horarios { get; set; }
        public DateTime CreadoUtc { get; set; }
        public DateTime ActualizadoUtc { get; set; }

        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public double Rating { get; set; }
        public string GooglePlaceId { get; set; }
    }

}
