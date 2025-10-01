using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Domain.Model
{
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

        public Usuario(string firebaseUid, string email, string nombre,string apellido,string idUsuario, string? fotoPerfilUrl = null)
        {
            FirebaseUid = firebaseUid ?? throw new ArgumentNullException(nameof(firebaseUid));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Apellido = apellido ?? throw new ArgumentNullException(nameof(nombre));
            IdUsuario = idUsuario ?? throw new ArgumentNullException(nameof(nombre));
            FotoPerfilUrl = fotoPerfilUrl;
        }
    }

}
