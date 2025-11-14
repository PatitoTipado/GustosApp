namespace GustosApp.API.DTO
{
    public class CrearOpinionRestauranteResponse
    {
        public Guid Id { get; set; }
        public int Valoracion {get; set; }
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
    
}
