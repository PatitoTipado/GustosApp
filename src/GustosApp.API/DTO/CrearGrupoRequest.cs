using System.ComponentModel.DataAnnotations;

namespace GustosApp.Application.DTO
{
    public class CrearGrupoRequest
    {
        [Required(ErrorMessage = "El nombre del grupo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string? Descripcion { get; set; }
    }
}
