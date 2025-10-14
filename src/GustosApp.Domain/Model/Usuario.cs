using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{

    public enum RegistroPaso { Ninguno = 0, Restricciones = 1, Condiciones = 2, Gustos = 3, Verificacion = 4, Finalizado = 5 }
    public class Usuario
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        // Identidad externa
        public string FirebaseUid { get; private set; }
        public string Email { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }

        public string IdUsuario { get; private set; }
        public string? FotoPerfilUrl { get; private set; }
        public DateTime FechaRegistro { get; private set; } = DateTime.UtcNow;
        public bool Activo { get; private set; } = true;

        public ICollection<Gusto> Gustos { get; set; } = new List<Gusto>();
        public ICollection<Restriccion> Restricciones { get; set; } = new List<Restriccion>();
        public ICollection<CondicionMedica> CondicionesMedicas { get; set; } = new List<CondicionMedica>();

        public RegistroPaso PasoActual { get; private set; } = RegistroPaso.Ninguno;

        // Relaciones con grupos
        public ICollection<Grupo> GruposAdministrados { get; set; } = new List<Grupo>();
        public ICollection<MiembroGrupo> MiembrosGrupos { get; set; } = new List<MiembroGrupo>();
        public ICollection<InvitacionGrupo> InvitacionesRecibidas { get; set; } = new List<InvitacionGrupo>();
        public ICollection<InvitacionGrupo> InvitacionesEnviadas { get; set; } = new List<InvitacionGrupo>();


        public void AvanzarPaso(RegistroPaso paso)
        {
            if ((int)paso >= (int)PasoActual) PasoActual = paso;
        }
        public Usuario(string firebaseUid, string email, string nombre, string apellido, string idUsuario, string? fotoPerfilUrl = null)
        {
            FirebaseUid = firebaseUid ?? throw new ArgumentNullException(nameof(firebaseUid));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Apellido = apellido ?? throw new ArgumentNullException(nameof(nombre));
            IdUsuario = idUsuario ?? throw new ArgumentNullException(nameof(nombre));
            FotoPerfilUrl = fotoPerfilUrl;
        }
        public virtual  List<string> ValidarCompatibilidad()
        {
            var gustosIncompatibles = new List<Gusto>();

            var tagsRestringidos = Restricciones
                .SelectMany(r => r.TagsProhibidos)
                .Select(t => t.NombreNormalizado)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);


            var tagsCondiciones = CondicionesMedicas
                .SelectMany(c => c.TagsCriticos)
                .Select(t => t.NombreNormalizado)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);


            var tagsProhibidos = tagsRestringidos
                .Union(tagsCondiciones)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var gusto in Gustos)
            {
                var tagsGusto = gusto.Tags.Select(t => t.NombreNormalizado);
                if (tagsGusto.Any(t => tagsProhibidos.Contains(t)))
                {
                    gustosIncompatibles.Add(gusto);
                }
            }

            foreach (var gustoIncompatible in gustosIncompatibles)
            {
                Gustos.Remove(gustoIncompatible);
            }

            return gustosIncompatibles.Select(g => g.Nombre).ToList();
        }


    }
}
