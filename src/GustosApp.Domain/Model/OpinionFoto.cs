using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class OpinionFoto
    {
        public Guid Id { get; set; }
        public Guid OpinionRestauranteId { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;

        public OpinionRestaurante Opinion { get; set; } = null!;
    }

}
