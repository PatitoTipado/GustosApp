using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class VotacionRestaurante
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid VotacionId { get; private set; }
        public Guid RestauranteId { get; private set; }

        // Navegación
        public VotacionGrupo Votacion { get; private set; }
        public Restaurante Restaurante { get; private set; }

        private VotacionRestaurante() { } // EF

        public VotacionRestaurante(Guid votacionId, Guid restauranteId)
        {
            VotacionId = votacionId;
            RestauranteId = restauranteId;
        }
    }

}
