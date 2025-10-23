using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class GrupoGusto
    {
        public Guid Id { get; set; }
        public Guid GrupoId { get; set; } // Debe coincidir con el tipo de Grupo.Id

        public Grupo Grupo { get; set; }

        public Guid GustoId { get; set; } // Debe coincidir con el tipo de Gusto.Id

        public Gusto Gusto { get; set; }
    }
}
