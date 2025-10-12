using System;

namespace GustosApp.Application.DTO
{
    public class SolicitudAmistadResponse
    {
        public Guid Id { get; set; }
        public UsuarioSimpleResponse Remitente { get; set; }
        public UsuarioSimpleResponse Destinatario { get; set; }
        public string Estado { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public string? Mensaje { get; set; }
    }
}
