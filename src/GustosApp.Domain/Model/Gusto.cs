using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class Gusto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }

        public string? ImagenUrl { get; set; }
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        public ICollection<Tag> Tags { get;  set; } = new List<Tag>();

        public ICollection <Restaurante> restaurantes { get; set; } = new List<Restaurante>();

        public ICollection<GrupoGusto> GruposRelacionados { get; set; } = new List<GrupoGusto>();
  
    
    
    
    }

   
}
