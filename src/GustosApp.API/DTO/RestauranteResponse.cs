namespace GustosApp.API.DTO
{
    public class RestauranteResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public double? Rating { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string ImagenUrl { get; set; } = string.Empty;
    }
}
