using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.API.DTO
{
    public class GuardarRestriccionesResponse()
    {
        public string Mensaje { get; set; }
        public List<string> GustosRemovidos { get; set; }
    }
}
