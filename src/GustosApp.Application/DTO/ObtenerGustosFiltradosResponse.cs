using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    public class ObtenerGustosFiltradosResponse
    {
        public List<GustoDto> GustosFiltrados { get; set; } = new();
        public List<Guid> GustosSeleccionados { get; set; } = new();
    }
    
    
}
