using System.Text.Json.Serialization;

namespace GustosApp.Application.DTO
{
    public class RegistrarUsuarioRequest
    {

        public string Nombre { get; set; }

        public string Apellido { get; set; }
        public string Email { get; set; }
        public string? FotoPerfilUrl { get; set; }
   
        public string Username { get; set; }


    }
}
