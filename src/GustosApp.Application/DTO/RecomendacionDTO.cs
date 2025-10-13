using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    public class RecomendacionDTO
    {
        public Guid RestaurantId { get; set; }
        public string Nombre { get; set; }
        public double Score { get; set; }

    }
}
