using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.Application.Model
{
    public class GustosFiltradosResult
    {
        public List<Gusto> GustosFiltrados { get; set; } = new();
        public List<Guid> GustosSeleccionados { get; set; } = new();
    }
}
