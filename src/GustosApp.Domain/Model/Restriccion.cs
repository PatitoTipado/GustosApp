using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class Restriccion
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        // lista de tags prohibidos (por ejemplo: harina, gluten, azúcar)

        public ICollection<Tag> TagsProhibidos { get;  set; } = new List<Tag>();

        public ICollection<Restaurante> Restaurantes { get; set; } = new List<Restaurante>();
    }

}
