namespace GustosApp.Application.DTOs.Restaurantes
{
    public class RestauranteDto
    {
        public Guid Id { get; set; }
        public string PropietarioUid { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string Direccion { get; set; } = "";
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public object? Horarios { get; set; }
        public DateTime CreadoUtc { get; set; }
        public DateTime ActualizadoUtc { get; set; }

        // V2
        public string Tipo { get; set; } = "";
        public string? ImagenUrl { get; set; }
        public decimal? Valoracion { get; set; }
        public List<string> Platos { get; set; } = new();
    }
}

