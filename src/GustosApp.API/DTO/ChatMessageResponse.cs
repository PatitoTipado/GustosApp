namespace GustosApp.API.DTO
{
    public class ChatMessageResponse
    {
        public Guid Id { get; set; }
        public Guid GrupoId { get; set; }
        public Guid UsuarioId { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
    }
}
