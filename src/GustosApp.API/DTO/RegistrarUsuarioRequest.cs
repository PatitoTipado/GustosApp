using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GustosApp.API.DTO
{
    public class RegistrarUsuarioRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio.")]public string Apellido { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]public string Email { get; set; }
        public string? FotoPerfilUrl { get; set; }
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")] public string Username { get; set; }


    }
}
