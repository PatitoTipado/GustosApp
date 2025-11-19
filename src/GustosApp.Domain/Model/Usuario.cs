using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{

    
    public enum PlanUsuario { Free = 0, Plus = 1 }
    public class Usuario
    {
    
        public Guid Id { get;  set; } = Guid.NewGuid();

        public bool EsPrivado { get; set; } = false;


        // Identidad externa
        public string FirebaseUid { get;  set; }
        public string Email { get;  set; }
        public string Nombre { get;  set; }
        public string Apellido { get;  set; }

        public string IdUsuario { get;  set; }
        public string? FotoPerfilUrl { get;  set; }
        public DateTime FechaRegistro { get;  set; } = DateTime.UtcNow;
        public bool Activo { get;  set; } = true;

        public bool RegistroInicialCompleto { get; set; }

        public PlanUsuario Plan { get; set; } = PlanUsuario.Free;


        public ICollection<Gusto> Gustos { get; set; } = new List<Gusto>();
        public ICollection<Restriccion> Restricciones { get; set; } = new List<Restriccion>();
        public ICollection<CondicionMedica> CondicionesMedicas { get; set; } = new List<CondicionMedica>();


        // Relaciones con grupos
        public ICollection<Grupo> GruposAdministrados { get; set; } = new List<Grupo>();
        public ICollection<MiembroGrupo> MiembrosGrupos { get; set; } = new List<MiembroGrupo>();
        public ICollection<InvitacionGrupo> InvitacionesRecibidas { get; set; } = new List<InvitacionGrupo>();
        public ICollection<InvitacionGrupo> InvitacionesEnviadas { get; set; } = new List<InvitacionGrupo>();
        public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();

        public bool EsPremium() => Plan == PlanUsuario.Plus;
        
        public void ActualizarAPlan(PlanUsuario nuevoPlan)
        {
            Plan = nuevoPlan;
        }
        public Usuario()
        {
            
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
        public ICollection<UsuarioRestauranteVisitado> Visitados { get; set; } = new List<UsuarioRestauranteVisitado>();
        public ICollection<UsuarioRestauranteFavorito> RestaurantesFavoritos { get; set; } = new List<UsuarioRestauranteFavorito>();
    }
}
