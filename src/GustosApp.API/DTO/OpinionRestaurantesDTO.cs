namespace GustosApp.API.DTO
{
    public class CrearValoracionResponse
    {
        public Guid Id { get; set; }
        public int ValoracionUsuario { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaCreacion { get; set; }

        public string? UsuarioNombre { get; set; }
        public string? UsuarioApellido { get; set; }

        public string? RestauranteNombre { get; set; }
        public double? RestauranteLatitud { get; set; }
        public double? RestauranteLongitud { get; set; }
    }
    public class CrearValoracionRequest
    {
        public Guid UsuarioId { get; set; }
        public Guid RestauranteId { get; set; }
        public int Valoracion { get; set; }
        public string? Comentario { get; set; }
    }
}
