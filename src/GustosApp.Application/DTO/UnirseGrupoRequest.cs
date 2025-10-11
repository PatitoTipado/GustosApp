using System.ComponentModel.DataAnnotations;

namespace GustosApp.Application.DTO
{
    public class UnirseGrupoRequest
    {
        [Required(ErrorMessage = "El código de invitación es obligatorio")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El código debe tener exactamente 8 caracteres")]
        public string CodigoInvitacion { get; set; } = string.Empty;
    }
}
