using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class UsuarioPreferencias
    {
        public List<string> Gustos { get; set; } = new();
        public List<string> Restricciones { get; set; } = new();
        public List<string> CondicionesMedicas { get; set; } = new();
    }

}
