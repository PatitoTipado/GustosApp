using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GustosApp.Domain.Model;

namespace GustosApp.API.DTO
{
    public class UsuarioResumenResponse
    {

        public string Nombre { get; set; } = null!;
        public string? Apellido { get; set; }

        public List<string> Gustos { get; set; } = new();
        public List<string> Restricciones { get; set; } = new();
        public List<string> CondicionesMedicas { get; set; } = new();

       public RegistroPaso PasoActual { get; set; }
     
    }
}