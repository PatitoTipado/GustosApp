using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class CondicionMedica
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = ""; // Ej: "Diabetes", "Hipertensión"
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
