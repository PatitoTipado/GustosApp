
namespace GustosApp.Application.DTOs.Restaurantes
{
    public class CrearRestauranteDto
    {
        public string Nombre { get; set; } = default!;
        public string Direccion { get; set; } = default!;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public object? Horarios { get; set; }

        // V2
        public string Tipo { get; set; } = default!;
        public List<string>? Platos { get; set; }
        public string? ImagenUrl { get; set; }
        public decimal? Valoracion { get; set; }
    }
}
