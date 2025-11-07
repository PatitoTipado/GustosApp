namespace GustosApp.API.DTO
{
    public class RestauranteListadoDto
    {
        public Guid Id { get; set; }
        public string PlaceId { get; set; } = string.Empty; // Google Places ID
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string? ImagenUrl { get; set; }

        public double? Rating { get; set; }
        public int? CantidadResenas { get; set; }

        // v1 New fields
        public string? PrimaryType { get; set; }
        public List<string>? Types { get; set; }
        public string? PriceLevel { get; set; }
        public bool? OpenNow { get; set; }

        // serves (subset)
        public bool? Delivery { get; set; }
        public bool? Takeout { get; set; }
        public bool? DineIn { get; set; }
        public bool? ServesVegetarianFood { get; set; }
        public bool? ServesCoffee { get; set; }
        public bool? ServesBeer { get; set; }
        public bool? ServesWine { get; set; }
    }
}
