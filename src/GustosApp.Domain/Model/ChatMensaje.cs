using System;

namespace GustosApp.Domain.Model
{
    public class ChatMensaje
    {
        public Guid Id { get; set; }
        public Grupo Grupo { get; set; }
        public Usuario? Usuario { get; set; }
        public string UsuarioNombre { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
        public Guid GrupoId { get; set; }
        public Guid? UsuarioId { get; set; }
    }
}
