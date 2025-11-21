using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class Grupo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nombre { get; private set; }
        public string? Descripcion { get; private set; }
        public Guid AdministradorId { get; private set; }
        public DateTime FechaCreacion { get; private set; } = DateTime.UtcNow;
        public bool Activo { get; private set; } = true;
        public string? CodigoInvitacion { get; private set; }
        public DateTime? FechaExpiracionCodigo { get; private set; }

        // Navegación
        public Usuario Administrador { get; set; }
        public ICollection<MiembroGrupo> Miembros { get; set; } = new List<MiembroGrupo>();
        public ICollection<InvitacionGrupo> Invitaciones { get; set; } = new List<InvitacionGrupo>();

        public ICollection<GrupoGusto> Gustos { get; set; } = new List<GrupoGusto>();
        private Grupo() { } // Para EF Core

        public Grupo(string nombre, Guid administradorId, string? descripcion = null)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            AdministradorId = administradorId;
            Descripcion = descripcion;
            GenerarCodigoInvitacion();
        }

        public void ActualizarInformacion(string nombre, string? descripcion = null)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del grupo no puede estar vacío", nameof(nombre));

            Nombre = nombre;
            Descripcion = descripcion;
        }

        public void GenerarCodigoInvitacion()
        {
            CodigoInvitacion = Guid.NewGuid().ToString("N")[..8].ToUpper();
            FechaExpiracionCodigo = DateTime.UtcNow.AddDays(7); // Código válido por 7 días
        }

        public void RenovarCodigoInvitacion()
        {
            GenerarCodigoInvitacion();
        }

        public bool EsCodigoInvitacionValido(string codigo)
        {
            return CodigoInvitacion == codigo && 
                   FechaExpiracionCodigo.HasValue && 
                   FechaExpiracionCodigo.Value > DateTime.UtcNow;
        }

        public void Desactivar()
        {
            Activo = false;
        }

        public void Activar()
        {
            Activo = true;
        }
    }
}
