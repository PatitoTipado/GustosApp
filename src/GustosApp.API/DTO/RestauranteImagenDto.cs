public class RestauranteImagenDto
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = string.Empty; // perfil|principal|interior|comida|menu
    public string Url { get; set; } = string.Empty;  
    public int? Orden { get; set; }
    public DateTime FechaCreacionUtc { get; set; }
    public string? MiniaturaUrl { get; set; }        
}