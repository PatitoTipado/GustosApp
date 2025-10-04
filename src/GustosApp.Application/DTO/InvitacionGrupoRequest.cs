using System.ComponentModel.DataAnnotations;

namespace GustosApp.Application.DTO
{
    public class InvitacionGrupoRequest
    {
        [Required(ErrorMessage = "El email del usuario a invitar es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string EmailUsuario { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "El mensaje personalizado no puede exceder los 200 caracteres")]
        public string? MensajePersonalizado { get; set; }
    }
}
