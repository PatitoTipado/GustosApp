using System.Text.Json.Serialization;

namespace GustosApp.API.DTO
{
    public class CrearOpinionRestauranteRequest
    {
        public Guid UsuarioId { get; set; }
        public Guid RestauranteId { get; set; }
        public int Valoracion { get; set; }
        public string? Opinion { get; set; }
        public string? Titulo { get; set; }
        public string? Img { get; set; }
        public DateTime FechaVisita { get; set; }
    }
}
