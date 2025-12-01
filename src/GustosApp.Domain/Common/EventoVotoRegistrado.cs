using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Common
{
    public class EventoVotoRegistrado
    {
        public Guid VotacionId { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = "";
        public string? UsuarioFirebaseUid { get; set; } = "";
        public string UsuarioFoto { get; set; } = "";
        public Guid RestauranteId { get; set; }
        public string RestauranteNombre { get; set; } = "";
        public string RestauranteImagen { get; set; } = "";

        public bool EsActualizacion{ get; set; }
}

}
