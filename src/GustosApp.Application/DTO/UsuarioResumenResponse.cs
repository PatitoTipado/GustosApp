using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    public class UsuarioResumenResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Apellido { get; set; }
        public string Email { get; set; } = null!;
        public string? FotoPerfilUrl { get; set; }

        public List<string> Gustos { get; set; } = new();
        public List<string> Restricciones { get; set; } = new();
        public List<string> CondicionesMedicas { get; set; } = new();
    }
}