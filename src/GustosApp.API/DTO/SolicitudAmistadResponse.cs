

namespace GustosApp.API.DTO
{
    public class SolicitudAmistadResponse
    {
        public Guid Id { get; set; }

        public UsuarioSimpleResponse Remitente { get; set; } = null!;
        public UsuarioSimpleResponse Destinatario { get; set; } = null!;

        public string Estado { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public string? Mensaje { get; set; }
    }
}
