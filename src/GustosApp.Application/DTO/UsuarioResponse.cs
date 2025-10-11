namespace GustosApp.Application.DTO
{
    public record UsuarioResponse(Guid Id, string FirebaseUid, string Email, string Nombre,string Apellido,string username, string? FotoPerfilUrl);

}
