namespace GustosApp.API.DTO
{
    public class CrearOpinionRestauranteRequest
    {
        public Guid RestauranteId { get; set; }
        public double Valoracion { get; set; }
        public string? Opinion { get; set; }
        public string? Titulo { get; set; }
        public DateTime? FechaVisita { get; set; }
        public string? MotivoVisita { get; set; }
        public List<IFormFile>? Imagenes { get; set; }
    }
    public class CrearOpinionRestauranteResponse
    {
        public Guid Id { get; set; }
        public int Valoracion { get; set; }
        public string? Opinion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Titulo { get; set; }
        public string? Img { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? UsuarioApellido { get; set; }
        public string? RestauranteNombre { get; set; }
        public double? RestauranteLatitud { get; set; }
        public double? RestauranteLongitud { get; set; }
    }
    public class OpinionRestauranteDto
    {
        public Guid Id { get; set; }
        public string Autor { get; set; } = "";
        public string Opinion { get; set; } = "";
        public int Valoracion { get; set; }
        public DateTime Fecha { get; set; }
        public string? ImagenAutor { get; set; }
        public List<string?>Fotos  { get; set; }
    }

}
