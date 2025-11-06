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
        public string Nombre { get; set; }
        public string? ImagenUrl { get; set; }
        public bool Seleccionado { get; set; } = false;

        public GustoDto() { }

        public GustoDto(Guid id, string nombre, string imagenUrl)
        {
            Id = id;
            Nombre = nombre;
            ImagenUrl = imagenUrl??"";
        }

    }
}
