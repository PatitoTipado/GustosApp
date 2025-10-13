using System.ComponentModel.DataAnnotations;

namespace GustosApp.Application.DTO
{
    public class InvitacionGrupoRequest
    {
        // Accept either an Email or a UsuarioId (GUID). Email is kept for backward compatibility.
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? EmailUsuario { get; set; }

    public Guid? UsuarioId { get; set; }

    // Also accept a username for lookup (buscar por username en lugar de email)
    public string? UsuarioUsername { get; set; }

        [StringLength(200, ErrorMessage = "El mensaje personalizado no puede exceder los 200 caracteres")]
        public string? MensajePersonalizado { get; set; }
    }
}
