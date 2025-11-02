using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
    public class MiembroGrupo
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid GrupoId { get; private set; }
        public Guid UsuarioId { get; private set; }
        public DateTime FechaUnion { get; private set; } = DateTime.UtcNow;
        public bool Activo { get; private set; } = true;
        public bool EsAdministrador { get; private set; }
        public bool afectarRecomendacion { get; set; } = true;
        public ICollection<GrupoGusto> GustosSeleccionados { get; set; } = new List<GrupoGusto>();

        // Navegación
        public Grupo Grupo { get; set; }
        public Usuario Usuario { get; set; }

        private MiembroGrupo() { } // Para EF Core

        public MiembroGrupo(Guid grupoId, Guid usuarioId, bool esAdministrador = false)
        {
            GrupoId = grupoId;
            UsuarioId = usuarioId;
            EsAdministrador = esAdministrador;
        }

        public void AbandonarGrupo()
        {
            Activo = false;
        }

        public void Reincorporar()
        {
            Activo = true;
            FechaUnion = DateTime.UtcNow; // Actualizar fecha de reincorporación
        }

        public void PromoverAAdministrador()
        {
            EsAdministrador = true;
        }

        public void DegradarAMiembro()
        {
            EsAdministrador = false;
        }
    }
}
