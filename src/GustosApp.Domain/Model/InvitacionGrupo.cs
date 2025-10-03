using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public enum EstadoInvitacion
    {
        Pendiente = 0,
        Aceptada = 1,
        Rechazada = 2,
        Expirada = 3
    }

    public class InvitacionGrupo
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid GrupoId { get; private set; }
        public Guid UsuarioInvitadoId { get; private set; }
        public Guid UsuarioInvitadorId { get; private set; }
        public DateTime FechaInvitacion { get; private set; } = DateTime.UtcNow;
        public DateTime? FechaRespuesta { get; private set; }
        public EstadoInvitacion Estado { get; private set; } = EstadoInvitacion.Pendiente;
        public string? MensajePersonalizado { get; private set; }
        public DateTime FechaExpiracion { get; private set; }

        // Navegación
        public Grupo Grupo { get; set; }
        public Usuario UsuarioInvitado { get; set; }
        public Usuario UsuarioInvitador { get; set; }

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
