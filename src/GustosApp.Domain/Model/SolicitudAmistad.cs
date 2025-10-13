using System;
namespace GustosApp.Domain.Model
{
    public enum EstadoSolicitud
    {
        Pendiente = 0,
        Aceptada = 1,
        Rechazada = 2,
        Expirada = 3
    }

    public class SolicitudAmistad
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid RemitenteId { get; private set; }
        public Guid DestinatarioId { get; private set; }
        public DateTime FechaEnvio { get; private set; } = DateTime.UtcNow;
        public DateTime? FechaRespuesta { get; private set; }
        public EstadoSolicitud Estado { get; private set; } = EstadoSolicitud.Pendiente;
        public string? Mensaje { get; private set; }

        // Navegación
        public Usuario Remitente { get; set; }
        public Usuario Destinatario { get; set; }

        private SolicitudAmistad() { }

        public SolicitudAmistad(Guid remitenteId, Guid destinatarioId, string? mensaje = null)
        {
            RemitenteId = remitenteId;
            DestinatarioId = destinatarioId;
            Mensaje = mensaje;
        }

        public void Aceptar()
        {
            if (Estado != EstadoSolicitud.Pendiente) throw new InvalidOperationException("Solo se pueden aceptar solicitudes pendientes");
            Estado = EstadoSolicitud.Aceptada;
            FechaRespuesta = DateTime.UtcNow;
        }

        public void Rechazar()
        {
            if (Estado != EstadoSolicitud.Pendiente) throw new InvalidOperationException("Solo se pueden rechazar solicitudes pendientes");
            Estado = EstadoSolicitud.Rechazada;
            FechaRespuesta = DateTime.UtcNow;
        }

        public void MarcarExpirada()
        {
            if (Estado == EstadoSolicitud.Pendiente)
            {
                Estado = EstadoSolicitud.Expirada;
                FechaRespuesta = DateTime.UtcNow;
            }
        }
    }
}
