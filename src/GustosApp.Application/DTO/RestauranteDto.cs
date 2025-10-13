namespace GustosApp.Application.DTO
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

        public double Rating { get; set; }
        public string GooglePlaceId { get; set; }

         // V2
        public string Tipo { get; set; } = default!;
       
        public string? ImagenUrl { get; set; }
        public decimal? Valoracion { get; set; }
        public List<string> Platos { get; set; } = new();
    }
}

