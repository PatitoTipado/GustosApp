using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class UserLocation
    {
    
        public double Lat { get; set; }
        public double Lng { get; set; }

        public int Radio{ get; set; }

        public DateTime Fecha { get; set; }


        public UserLocation(double lat, double lng, int radio,DateTime fecha)
        {
            Lat = lat;
            Lng = lng;
            Radio = radio;
            Fecha = fecha;

        }
    }
}
