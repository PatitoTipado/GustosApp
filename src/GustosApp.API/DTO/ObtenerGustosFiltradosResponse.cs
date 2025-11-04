using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Application.DTO;

namespace GustosApp.API.DTO
{
    public class ObtenerGustosFiltradosResponse
    {
        public string PasoActual { get; set; } = string.Empty;
        public string Next { get; set; } = string.Empty;
        public List<GustoDto> Gustos { get; set; } = new();

    }
    }
