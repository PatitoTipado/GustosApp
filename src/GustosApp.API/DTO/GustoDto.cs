using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace GustosApp.API.DTO
    {
        
        public class GustoDto
        {
            public Guid Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string? ImagenUrl { get; set; }
            public bool Seleccionado { get; set; } = false;

            public GustoDto() { }

            public GustoDto(Guid id, string nombre, string? imagenUrl = null)
            {
                Id = id;
                Nombre = nombre;
                ImagenUrl = imagenUrl;
            }
        }

    
        public class ObtenerGustosFiltradosResponse
        {
            public string PasoActual { get; set; } = string.Empty;
            public string Next { get; set; } = string.Empty;
            public List<GustoDto> Gustos { get; set; } = new();
        }

    public class GuardarGustosResponse
    {
        public string Mensaje { get; set; }
        public List<string> GustosIncompatibles { get; set; }
    }


}
