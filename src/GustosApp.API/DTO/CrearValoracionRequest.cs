namespace GustosApp.API.DTO
{
    public class CrearValoracionRequest
    {
        public Guid UsuarioId { get; set; }
        public Guid RestauranteId { get; set; }
        public int Valoracion { get; set; }
        public string? Comentario { get; set; }
    }
}
