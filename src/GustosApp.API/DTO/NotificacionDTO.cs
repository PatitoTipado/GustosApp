using GustosApp.Domain.Model.@enum;

namespace GustosApp.API.DTO
{
    public class NotificacionDTO
    {
        public Guid Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Mensaje { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public bool Leida { get; set; } = false;

        public DateTime FechaCreacion { get; set; }
    }

    public class CrearNotificacionRequest
    {
        public Guid UsuarioDestinoId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public TipoNotificacion TipoNotificacion { get; set; }
        public string nombreUsuario { get; set; } = string.Empty;
        public string? nombreGrupo { get; set; } // opcional


    }
}

