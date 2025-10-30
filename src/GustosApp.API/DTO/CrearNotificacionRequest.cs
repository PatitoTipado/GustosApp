using GustosApp.Domain.Model;
using Microsoft.AspNetCore.Mvc;

namespace GustosApp.API.DTO
{
    public class CrearNotificacionRequest
    {
        public Guid UsuarioDestinoId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public TipoNotificacion TipoNotificacion { get; set; }
        public string nombreUsuario { get; set; } = string.Empty;
        public string? nombreGrupo { get; set; } // opcional


    }
}
