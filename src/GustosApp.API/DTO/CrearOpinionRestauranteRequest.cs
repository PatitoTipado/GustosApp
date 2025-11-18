using System.Text.Json.Serialization;

namespace GustosApp.API.DTO
{
    public class CrearOpinionRestauranteRequest
    {
        public Guid RestauranteId { get; set; }
        public int Valoracion { get; set; }
        public string? Opinion { get; set; }
        public string? Titulo { get; set; }
        public DateTime? FechaVisita { get; set; }
        public string? MotivoVisita { get; set; }
        public List<IFormFile>? Imagenes { get; set; }
    }
}
