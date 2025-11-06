namespace GustosApp.API.DTO
{
    public class EliminarAmigoResponse
    {
        public bool Success { get; set; }
       
        public string Message { get; set; } = string.Empty;

        public UsuarioSimpleResponse? AmigoEliminado { get; set; }
    }
}
