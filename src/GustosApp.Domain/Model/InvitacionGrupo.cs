using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
   

    public class InvitacionGrupo
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        public Guid? NotificacionId { get; set; }
        public Guid GrupoId { get;  set; }
        public Guid UsuarioInvitadoId { get;set; }
        public Guid UsuarioInvitadorId { get;  set; }
        public DateTime FechaInvitacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaRespuesta { get; set; }
        public EstadoInvitacion Estado { get; set; } = EstadoInvitacion.Pendiente;
        public string? MensajePersonalizado { get; set; }
        public DateTime FechaExpiracion { get; set; }

        // Navegación
        public Grupo Grupo { get; set; }
        public Usuario UsuarioInvitado { get; set; }
        public Usuario UsuarioInvitador { get; set; }

        public Notificacion? Notificacion { get; set; }

        private InvitacionGrupo() { } // Para EF Core

        public InvitacionGrupo(Guid grupoId, Guid usuarioInvitadoId, Guid usuarioInvitadorId, string? mensajePersonalizado = null)
        {
            GrupoId = grupoId;
            UsuarioInvitadoId = usuarioInvitadoId;
            UsuarioInvitadorId = usuarioInvitadorId;
            MensajePersonalizado = mensajePersonalizado;
            FechaExpiracion = DateTime.UtcNow.AddDays(7); // Invitación válida por 7 días
        }

        public void Aceptar()
        {
            if (Estado != EstadoInvitacion.Pendiente)
                throw new InvalidOperationException("Solo se pueden aceptar invitaciones pendientes");

            if (FechaExpiracion < DateTime.UtcNow)
            {
                Estado = EstadoInvitacion.Expirada;
                throw new InvalidOperationException("La invitación ha expirado");
            }

            Estado = EstadoInvitacion.Aceptada;
            FechaRespuesta = DateTime.UtcNow;
        }

        public void Rechazar()
        {
            if (Estado != EstadoInvitacion.Pendiente)
                throw new InvalidOperationException("Solo se pueden rechazar invitaciones pendientes");

            Estado = EstadoInvitacion.Rechazada;
            FechaRespuesta = DateTime.UtcNow;
        }

        public void MarcarComoExpirada()
        {
            if (Estado == EstadoInvitacion.Pendiente)
            {
                Estado = EstadoInvitacion.Expirada;
                FechaRespuesta = DateTime.UtcNow;
            }
        }

        public bool EstaExpirada()
        {
            return FechaExpiracion < DateTime.UtcNow && Estado == EstadoInvitacion.Pendiente;
        }
    }
}
