using System.Text.Json;

public class RestauranteMenuDto
{
    public Guid Id { get; set; }
    public Guid RestauranteId { get; set; }
    public string Moneda { get; set; } = "ARS";
    public int Version { get; set; } = 1;
    public DateTime FechaActualizacionUtc { get; set; }

    public JsonElement? Menu { get; set; }
    public string? MenuRaw { get; set; }
}