using GustosApp.Domain.Model;

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
}

