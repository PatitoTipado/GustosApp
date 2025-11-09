using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.API.DTO
{

    public class RestriccionDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Seleccionado { get; set; }
    }
    public class RestriccionResponse
        {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Seleccionado { get; set; }


        public RestriccionResponse(Guid id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
    }
    public class GuardarRestriccionesResponse
    {
        public string Mensaje { get; set; }
        public List<string> GustosRemovidos { get; set; }
    }

}
