using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.API.DTO
{
    public class CondicionMedicaResponse
    {

        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Seleccionado { get; set; }

    }
}
