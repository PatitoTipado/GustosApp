using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
 
    public class Notificacion
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UsuarioDestinoId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Mensaje { get; set; } = string.Empty;

        public TipoNotificacion Tipo { get; set; }

        public bool Leida { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación opcional con Usuario (si la tenés)
        public Usuario? UsuarioDestino { get; set; }
    }
}
