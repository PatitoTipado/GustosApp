

namespace GustosApp.API.DTO
{
    public class UsuarioSimpleResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public string Username { get; set; }
    }
}
