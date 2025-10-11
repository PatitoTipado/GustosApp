
namespace GustosApp.Application.DTOs.Restaurantes
{
    public class CrearRestauranteDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public object? Horarios { get; set; } // se serializa tal cual a JSON
    }
}
