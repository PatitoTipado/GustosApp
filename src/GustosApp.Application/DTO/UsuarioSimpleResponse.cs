using System;

namespace GustosApp.Application.DTO
{
    public class UsuarioSimpleResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string? FotoPerfilUrl { get; set; }
        public string username { get; set; }
    }
}
